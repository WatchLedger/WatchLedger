using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WatchCollection.Api.Middleware;
using WatchCollection.Api.Services;
using WatchCollection.Domain.Services;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Infrastructure;
using WatchCollection.Shared.Converters;
using WatchCollection.Storage;
using WatchCollection.Storage.Entities.Data;
using WatchCollection.Storage.Interfaces;


namespace WatchCollection.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Azure Key Vault
        var keyVaultUri = builder.Configuration["AzureKeyVault:VaultUri"]
            ?? throw new InvalidOperationException("Azure Key Vault URI is not configured.");
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential());

        // Configure services using extension methods
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);
        builder.Services.AddApplicationServices();
        builder.Services.AddApiConfiguration();
        builder.Services.AddCorsPolicy();
        builder.Services.AddRateLimiting();

        var app = builder.Build();

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.MapOpenApi();
        app.MapScalarApiReference();
        app.UseCors();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRateLimiter();
        app.MapControllers();
        app.Run();
    }
}