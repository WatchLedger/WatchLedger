using System.Text.Json.Serialization;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
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

        var keyVaultUri = builder.Configuration["AzureKeyVault:VaultUri"]
            ?? throw new InvalidOperationException("Azure Key Vault URI is not configured.");
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential());

        builder.Services.Configure<BlobStorageOptions>( options =>{
            options.BlobStorageConnectionString = builder.Configuration["BlobConnectionString"]
                ?? throw new InvalidOperationException("Blob storage connection string is not configured in Key Vault.");
            options.ContainerName = builder.Configuration["BlobStorage:ContainerName"]
                ?? throw new InvalidOperationException("Blob storage container name is not configured.");
        });

        var connectionString = builder.Configuration["ProductionSqlString"]
            ?? throw new InvalidOperationException("SQL Database connection string is not configured in Key Vault.");
        builder.Services.AddDbContext<WatchServiceDbContext>(options => 
            options.UseSqlServer(connectionString));

        // Add services to the container.
        builder.Services.AddScoped<IWatchService, WatchService>();
        builder.Services.AddScoped<IWatchImageService, WatchImageService>();
        builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();
        builder.Services.AddScoped<IBidService, BidService>();
        builder.Services.AddHttpClient<IWatchValuationHttpClient, WatchValuationHttpClient>();
        // Add repositories to the container.
        builder.Services.AddScoped<IWatchRepository, WatchRepository>();
        builder.Services.AddScoped<IWatchImageRepository, WatchImageRepository>();
        builder.Services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();
        builder.Services.AddScoped<IBidRepository, BidRepository>();
        builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new WatchConditionJsonConverter());
            });
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}