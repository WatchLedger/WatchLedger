using Azure.Identity;
using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using IdentityServer.Data;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityServer;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        var connectionString = builder.Configuration["IdentityServerProductionSqlString"]
            ?? throw new InvalidOperationException("SQL Database connection string is not configured in Key Vault.");
        
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddDbContext<ConfigurationDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        var migrationsAssembly = typeof(Program).Assembly.GetName().Name;
        
        builder.Services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
            })
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlServer(
                    connectionString,
                    sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            .AddAspNetIdentity<ApplicationUser>()
            .AddProfileService<ProfileService>();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();


        //app.InitializeDatabase();
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}