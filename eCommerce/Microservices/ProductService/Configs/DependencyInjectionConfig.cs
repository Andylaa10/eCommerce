using ProductService.Core.Helpers;
using ProductService.Core.Repositories;
using ProductService.Core.Repositories.Interfaces;
using ProductService.Core.Services.Interfaces;

namespace ProductService.Configs;

public static class DependencyInjectionConfig
{
    public static void ConfigureDi(this IServiceCollection services)
    {
        // DB
        services.AddDbContext<DatabaseContext>();
        
        // DI
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, Core.Services.ProductService>();
        
        // Automapper
        services.AddSingleton(AutoMapperConfig.ConfigureAutoMapper());
    }
}