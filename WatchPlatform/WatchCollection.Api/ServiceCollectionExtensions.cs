using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WatchCollection.Api.Services;
using WatchCollection.Domain.Services;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Infrastructure;
using WatchCollection.Shared.Converters;
using WatchCollection.Storage;
using WatchCollection.Storage.Entities.Data;
using WatchCollection.Storage.Interfaces;

namespace WatchCollection.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BlobStorageOptions>(options =>
        {
            options.BlobStorageConnectionString = configuration["BlobConnectionString"]
                ?? throw new InvalidOperationException("Blob storage connection string is not configured in Key Vault.");
            options.ContainerName = configuration["BlobStorage:ContainerName"]
                ?? throw new InvalidOperationException("Blob storage container name is not configured.");
        });

        var connectionString = configuration["ProductionSqlString"]
            ?? throw new InvalidOperationException("SQL Database connection string is not configured in Key Vault.");
        services.AddDbContext<WatchServiceDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IAuthorizationHandler, AuthHandler>();
        services.AddScoped<IUserRoleService, UserRoleService>();

        services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.Authority = "https://identityserver-watchcollection.azurewebsites.net";
                options.TokenValidationParameters.ValidateAudience = false;
                options.MapInboundClaims = false;
                options.TokenValidationParameters.RoleClaimType = "role";
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("PublicReadPolicy", policy =>
            {
                policy.Requirements.Add(new ClaimOrRoleRequirement("WatchCollection.Api.Read", null));
            })
            .AddPolicy("CollectionReadPolicy", policy =>
            {
                policy.Requirements.Add(new ClaimOrRoleRequirement("WatchCollection.Api.Read", "User"));
            })
            .AddPolicy("CollectionWritePolicy", policy =>
            {
                policy.Requirements.Add(new ClaimOrRoleRequirement("WatchCollection.Api.Write", "User"));
            })
            .AddPolicy("AdminWritePolicy", policy =>
            {
                policy.Requirements.Add(new ClaimOrRoleRequirement("WatchCollection.Api.Write", "Admin"));
            })
            .AddPolicy("AdminOrUserWritePolicy", policy =>
            {
                policy.Requirements.Add(new ClaimOrRoleRequirement("WatchCollection.Api.Write", "AdminOrUser"));
            });

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Domain services
        services.AddScoped<IWatchService, WatchService>();
        services.AddScoped<IWatchImageService, WatchImageService>();
        services.AddScoped<IAdvertisementService, AdvertisementService>();
        services.AddScoped<IBidService, BidService>();
        services.AddHttpClient<IWatchValuationHttpClient, WatchValuationHttpClient>();

        // Repositories
        services.AddScoped<IWatchRepository, WatchRepository>();
        services.AddScoped<IWatchImageRepository, WatchImageRepository>();
        services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();
        services.AddScoped<IBidRepository, BidRepository>();
        services.AddScoped<IBlobStorageService, BlobStorageService>();

        return services;
    }

    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // The baseline all requests are limited to 100/min per user/IP
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var userId = context.User.FindFirst("sub")?.Value
                        ?? context.Connection.RemoteIpAddress?.ToString()
                        ?? "anonymous";
                return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1)
                });
            });

            // Image uploads heavily restricted due to Blob Limitations (5 requests allowed per minute)
            options.AddPolicy("imageUpload", context =>
            {
                var userId = context.User.FindFirst("sub")?.Value ?? "anonymous";
                return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1)
                });
            });

            // Adding bids is restricted to avoid spam (5 requests allowed per minute)
            options.AddPolicy("bidSubmission", context =>
            {
                var userId = context.User.FindFirst("sub")?.Value ?? "anonymous";
                return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1)
                });
            });

            // Passthrough to get brandlist from valuationservice is limited (200 requests allowed per minute)
            options.AddPolicy("brandList", context =>
            {
                var userId = context.User.FindFirst("sub")?.Value ?? "anonymous";
                return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 200,
                    Window = TimeSpan.FromMinutes(1)
                });
            });

            // Valuation requests are limited due to link with external service (5 requests allowed per minute)
            options.AddPolicy("watchValuation", context =>
            {
                var userId = context.User.FindFirst("sub")?.Value ?? "anonymous";
                return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1)
                });
            });
        });

        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:5174", "https://watchledger.nathangeleyn.com")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new WatchConditionJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new AdvertisementStatusJsonConverter());
            });

        services.AddOpenApi();

        return services;
    }
}
