﻿using Cache;
using CartService.Core.Helpers;
using CartService.Core.Helpers.MessageHandlers;
using CartService.Core.Repositories;
using CartService.Core.Repositories.Interfaces;
using CartService.Core.Services.Interfaces;
using Messaging;
using MonitoringService;
using OpenTelemetry.Trace;

namespace CartService.Configs;

public static class DependencyInjectionConfig
{
    public static void ConfigureDependencyInjection(this IServiceCollection services)
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

        // Messaging 
        services.AddSingleton(new MessageClient());

        // MessageHandler
        services.AddHostedService<CreateCartMessageHandler>();
        services.AddHostedService<DeleteCartMessageHandler>();
        services.AddHostedService<UpdateCartMessageHandler>();

        // Monitoring
        var serviceName = "CartService";
        var serviceVersion = "1.0.0";
        services.AddOpenTelemetry().Setup(serviceName, serviceVersion);
        services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));
    }
}