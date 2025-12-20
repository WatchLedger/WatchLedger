using Microsoft.EntityFrameworkCore;
using WatchCollection.Domain.Services;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage;
using WatchCollection.Storage.Entities.Data;
using WatchCollection.Storage.Interfaces;


namespace WatchCollection.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("WatchCollection");
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
        builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

        builder.Services.AddControllers();
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