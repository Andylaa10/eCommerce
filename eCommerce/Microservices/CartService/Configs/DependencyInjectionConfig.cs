using Cache;
using CartService.Core.Helpers;
using CartService.Core.Repositories;
using CartService.Core.Repositories.Interfaces;
using CartService.Core.Services.Interfaces;
using MonitoringService;
using OpenTelemetry.Trace;

namespace CartService.Configs;

public static class DependencyInjectionConfig
{
    public static void ConfigureDi(this IServiceCollection services)
    {
        // DB
        services.AddDbContext<DatabaseContext>();

        // DI
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartService, Core.Services.CartService>();

        // Automapper
        services.AddSingleton(AutoMapperConfig.ConfigureAutoMapper());

        // Caching
        services.AddSingleton(RedisClientFactory.CreateRedisClient());

        // Monitoring
        var serviceName = "CartService";
        var serviceVersion = "1.0.0";
        services.AddOpenTelemetry().Setup(serviceName, serviceVersion);
        services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));
    }
}