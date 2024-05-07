using AuthService.Core.Entities;
using AuthService.Core.Services.DTOs;
using AutoMapper;

namespace AuthService.Configs;

public static class AutoMapperConfig
{
    public static IMapper ConfigureAutomapper()
    {
        var mapper = new MapperConfiguration(options =>
        {
            // DTO to Entity
            options.CreateMap<CreateAuthDto, Auth>();
        }).CreateMapper();

        return mapper;
    }
}