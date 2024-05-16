using Cache;
using EasyNetQ;
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
    public static void ConfigureDependencyInjection(this IServiceCollection services)
    {
        // DB
        services.AddDbContext<DatabaseContext>();
        
        // DI
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, Core.Services.ProductService>();
        
        // Automapper
        services.AddSingleton(AutoMapperConfig.ConfigureAutoMapper());
        
        // Caching
        services.AddSingleton(RedisClientFactory.CreateRedisClient());
        
        // Messaging 
        const string connectionStringRabbitMq = "host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"; // TODO
        services.AddSingleton(new MessageClient(RabbitHutch.CreateBus(connectionStringRabbitMq)));
        
        // Monitoring
        var serviceName = "PatientService";
        var serviceVersion = "1.0.0"; 
        services.AddOpenTelemetry().Setup(serviceName, serviceVersion);
        services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));
    }
}