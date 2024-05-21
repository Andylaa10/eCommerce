using AuthService.Core.Helpers;
using AuthService.Core.Helpers.MessageHandlers;
using AuthService.Core.Repositories;
using AuthService.Core.Repositories.Interfaces;
using AuthService.Core.Services.Interfaces;
using Messaging;
using MonitoringService;
using OpenTelemetry.Trace;

namespace AuthService.Configs;

public static class DependencyInjectionConfig
{
    public static void ConfigureDependencyInjection(this IServiceCollection services)
    {
        // DB 
        services.AddDbContext<DatabaseContext>();
        
        // DI
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IAuthService, Core.Services.AuthService>();

        // Messaging 
        services.AddSingleton(new MessageClient());
        
        // MessageHandlers
        services.AddHostedService<DeleteAuthMessageHandler>();
        
        // Monitoring
        var serviceName = "AuthService";
        var serviceVersion = "1.0.0";
        services.AddOpenTelemetry().Setup(serviceName, serviceVersion);
        services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));
    }
}