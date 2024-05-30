using Cache;
using Messaging;
using MonitoringService;
using OpenTelemetry.Trace;
using ProductService.Core.Helpers;
using ProductService.Core.Repositories;
using ProductService.Core.Repositories.Interfaces;
using ProductService.Core.Services.Interfaces;

namespace ProductService.Configs;

public static class DependencyInjectionConfig
{
    public static void ConfigureDependencyInjection(this IServiceCollection services, WebApplicationBuilder builder)
    {
        // DB
        services.AddDbContext<DatabaseContext>();
        
        // DI
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, Core.Services.ProductService>();
        
        // Caching
        // services.AddScoped<IRedisClient, RedisClient>();
        
        // Automapper
        services.AddSingleton(AutoMapperConfig.ConfigureAutoMapper());
        
        // Configure AppSettings
        services.Configure<AppSettings.AppSettings>(builder.Configuration.GetSection("AppSettings"));
        
        // Messaging 
        services.AddSingleton(new MessageClient());
        
        // Monitoring
        var serviceName = "ProductService";
        var serviceVersion = "1.0.0"; 
        services.AddOpenTelemetry().Setup(serviceName, serviceVersion);
        services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));
    }
}