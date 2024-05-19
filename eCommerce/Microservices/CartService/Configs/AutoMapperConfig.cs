using AutoMapper;
using CartService.Core.Entities;
using CartService.Core.Services.DTOs;

namespace CartService.Configs;

public static class AutoMapperConfig
{
    public static IMapper ConfigureAutoMapper()
    {
        var mapperConfig = new MapperConfiguration(config =>
        {
            //DTO to entity
            config.CreateMap<CreateCartDto, Cart>();
            config.CreateMap<AddProductToCartDto, ProductLine>();
            config.CreateMap<UpdateCartDto, Cart>();
        });

        return mapperConfig.CreateMapper();
    }
}