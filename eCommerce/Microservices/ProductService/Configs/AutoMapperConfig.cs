using AutoMapper;
using ProductService.Core.Entities;
using ProductService.Core.Services.DTOs;

namespace ProductService.Configs;

public class AutoMapperConfig
{
    public static IMapper ConfigureAutoMapper()
    {
        var mapperConfig = new MapperConfiguration(config =>
        {
            //DTO to entity
            config.CreateMap<CreateProductDto, Product>();
            config.CreateMap<UpdatedProductDto, Product>();
        });

        return mapperConfig.CreateMapper();
    }
}