using WatchValuation.Domain.Services;
using WatchValuation.Domain.Services.Interfaces;
using Microsoft.AspNetCore.RateLimiting;
using Azure.Identity;
using WatchValuation.Storage;
using WatchValuation.Storage.Interfaces;
using WatchValuation.Domain.Services.Exceptions;
using Polly;
using Polly.Extensions.Http;
using System.Net;

namespace WatchValuation.Api;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add Azure Key Vault configuration
        builder.Configuration.AddAzureKeyVault(
            new Uri("https://watchplatform-keyvault.vault.azure.net/"),
            new DefaultAzureCredential());

        // Add services to the container.
        builder.Services.AddScoped<IValuationService, ValuationService>();
        builder.Services.AddScoped<IBrandsService, BrandsService>();
        builder.Services.AddScoped<IValuationCacheRepository, ValuationCacheRepository>();
        builder.Services.AddScoped<IBrandsCacheRepository, BrandsCacheRepository>();
        builder.Services
            .AddHttpClient<ValuationService>()
            .AddPolicyHandler(GetRetryPolicy());
        builder.Services
            .AddHttpClient<BrandsService>()
            .AddPolicyHandler(GetRetryPolicy());

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        // Add rate limiting
        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("valuation", limiterOptions =>
            {
                limiterOptions.PermitLimit = 100;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
            });

            options.AddFixedWindowLimiter("brands", limiterOptions =>
            {
                limiterOptions.PermitLimit = 1000;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
            });
        });

        // Add authorization
        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseRateLimiter();
        app.UseAuthorization();

        var valuationGroup = app.MapGroup("/api")
            .WithName("Valuation")
            .WithOpenApi();

        valuationGroup.MapGet("/valuation", GetValuation)
            .WithName("GetValuation")
            .WithDescription("Get valuation for a watch")
            .RequireRateLimiting("valuation");

        valuationGroup.MapGet("/brands", GetBrands)
            .WithName("GetBrands")
            .WithDescription("Get list of available watch brands")
            .RequireRateLimiting("brands");

        app.Run();
    }

    private static async Task<IResult> GetValuation(IValuationService service, string referenceNumber)
    {
        try
        {
            var result = await service.GetWatchValuationFromCacheAsync(referenceNumber);
            return Results.Ok(result);
        }
        catch (WatchValuationUnavailableException ex)
        {
            return Results.Ok(new { ex.Message });
        }
        catch (WatchValuationException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (Exception)
        {
            return  Results.Problem("An error occurred while retrieving the valuation.");
        }
    }

    private static async Task<IResult> GetBrands(IBrandsService service)
    {
        try
        {
            var result = await service.GetWatchBrandsFromCacheAsync();
            return Results.Ok(result);
        }
        catch (WatchBrandListException ex)
        {
            return Results.Problem(ex.Message);
        }
        catch (Exception)
        {
            return Results.Problem("An error occurred while retrieving brands.");
        }
    }
}