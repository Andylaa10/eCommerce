﻿using Cache;
using Messaging;
using MonitoringService;
using OpenTelemetry.Trace;
using UserService.Core.Helpers;
using UserService.Core.Helpers.MessageHandlers;
using UserService.Core.Repositories;
using UserService.Core.Repositories.Interfaces;
using UserService.Core.Services.Interfaces;

namespace UserService.Configs;

public static class DependencyInjectionConfig
{
    public static void ConfigureDependencyInjection(this IServiceCollection services)
    {
        // DB
        services.AddDbContext<DatabaseContext>();
        
        // DI
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, Core.Services.UserService>();
        
        // Automapper
        services.AddSingleton(AutoMapperConfig.ConfigureAutoMapper());
        
        // Caching
        services.AddSingleton(RedisClientFactory.CreateRedisClient());

        // Messaging 
        services.AddSingleton(new MessageClient());
        
        // MessageHandler 
        services.AddHostedService<CreateUserMessageHandler>();
       
        // Monitoring
        var serviceName = "UserService";
        var serviceVersion = "1.0.0";
        services.AddOpenTelemetry().Setup(serviceName, serviceVersion);
        services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));
    }
}