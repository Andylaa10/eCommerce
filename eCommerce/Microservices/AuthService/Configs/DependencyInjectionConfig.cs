using AuthService.Core.Helpers;
using AuthService.Core.Repositories;
using AuthService.Core.Repositories.Interfaces;
using AuthService.Core.Services.Interfaces;
using EasyNetQ;
using Messaging;

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
        const string connectionStringRabbitMq = "host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"; // TODO
        services.AddSingleton(new MessageClient(RabbitHutch.CreateBus(connectionStringRabbitMq)));
    }
}