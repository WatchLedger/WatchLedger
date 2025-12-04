namespace WatchValuation.Api;
using WatchValuation.Domain.Services;
using WatchValuation.Domain.Services.Interfaces;
using WatchValuation.Api.Contracts;
using Microsoft.AspNetCore.RateLimiting;
using Azure.Identity;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add Azure Key Vault configuration
        var keyVaultUrl = builder.Configuration["KeyVault:Url"];
        if (!string.IsNullOrEmpty(keyVaultUrl))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUrl),
                new DefaultAzureCredential());
        }

        // Add services to the container.
        builder.Services.AddScoped<IValuationService, ValuationService>();
        builder.Services.AddScoped<IBrandsService, BrandsService>();
        builder.Services.AddHttpClient<ValuationService>();
        builder.Services.AddHttpClient<BrandsService>();

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

        valuationGroup.MapPost("/valuation", GetValuation)
            .WithName("GetValuation")
            .WithDescription("Get valuation for a watch")
            .RequireRateLimiting("valuation");

        valuationGroup.MapGet("/brands", GetBrands)
            .WithName("GetBrands")
            .WithDescription("Get list of available watch brands")
            .RequireRateLimiting("brands");

        app.Run();
    }

    private static async Task<IResult> GetValuation(IValuationService service, ValuationRequestContract request)
    {
        try
        {
            var result = await service.GetValuation(request);
            return Results.Ok(result);
        }
        catch (Exception)
        {
            return  Results.BadRequest("An error occurred while retrieving the valuation.");
        }
    }

    private static async Task<IResult> GetBrands(IBrandsService service)
    {
        try
        {
            var result = await service.GetBrands();
            return Results.Ok(result);
        }
        catch (Exception)
        {
            return Results.BadRequest("An error occurred while retrieving brands.");
        }
    }
}