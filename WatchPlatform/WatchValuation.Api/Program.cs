using WatchValuation.Domain.Services;
using WatchValuation.Domain.Services.Interfaces;
using Microsoft.AspNetCore.RateLimiting;
using Azure.Identity;
using WatchValuation.Storage;
using WatchValuation.Storage.Interfaces;
using WatchValuation.Domain.Services.Exceptions;
using WatchValuation.Infrastructure;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WatchValuation.Api;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var keyVaultUri = builder.Configuration["AzureKeyVault:VaultUri"]
            ?? throw new InvalidOperationException("Azure Key Vault URI is not configured.");
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential());


        builder.Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.Authority = "https://localhost:5001";
                options.TokenValidationParameters.ValidateAudience = false;
            });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("ValuationReadPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "WatchValuation.Api.Read");
            });

        builder.Services.Configure<ExternalApiOptions>(options =>
        {
            options.BaseUrl = builder.Configuration["ExternalApi:BaseUrl"]
                ?? throw new InvalidOperationException("ExternalApi BaseUrl is not configured.");
            options.ApiKey = builder.Configuration["TheWatchApi"]
                ?? throw new InvalidOperationException("WatchApi key is not configured in Key Vault.");
        });

        builder.Services.Configure<CosmosDbOptions>(options =>
        {
            options.CosmosConnectionString = builder.Configuration["watchplatformcache-connectionstring"]
                ?? throw new InvalidOperationException("Cosmos DB connection string is not configured in Key Vault.");
            options.DatabaseName = builder.Configuration["CosmosDb:DatabaseName"]
                ?? throw new InvalidOperationException("Cosmos DB database name is not configured.");
            options.ContainerName = builder.Configuration["CosmosDb:ContainerName"]
                ?? throw new InvalidOperationException("Cosmos DB container name is not configured.");
        });


        builder.Services.AddScoped<IValuationService, ValuationService>();
        builder.Services.AddScoped<IBrandsService, BrandsService>();
        builder.Services.AddSingleton<ICosmosContainerProvider, CosmosContainerProvider>();
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

        // Add rate limiting to the endpoints
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

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi("/openapi/testen");
        }

        app.UseRateLimiter();
        app.UseAuthorization();

        var valuationGroup = app.MapGroup("/api")
            .WithName("Valuation")
            .WithOpenApi();

        valuationGroup.MapGet("/valuation", GetValuation)
            .WithName("GetValuation")
            .WithDescription("Get valuation for a watch")
            .RequireRateLimiting("valuation")
            .RequireAuthorization("ValuationReadPolicy");

        valuationGroup.MapGet("/brands", GetBrands)
            .WithName("GetBrands")
            .WithDescription("Get list of available watch brands")
            .RequireRateLimiting("brands")
            .RequireAuthorization("ValuationReadPolicy");

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