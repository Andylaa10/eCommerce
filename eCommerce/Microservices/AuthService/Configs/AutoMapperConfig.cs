using AuthService.Core.Entities;
using AuthService.Core.Services.DTOs;
using AutoMapper;

namespace AuthService.Configs;

public static class AutoMapperConfig
{
    public static IMapper ConfigureAutoMapper()
    {
        var mapper = new MapperConfiguration(options =>
        {
            // DTO to Entity
            options.CreateMap<GetAuthDto, Auth>();
            options.CreateMap<CreateAuthDto, Auth>();
            
            // Entity to DTO
            options.CreateMap<Auth, GetAuthDto>();

        }).CreateMapper();

        return mapper;
    }
}