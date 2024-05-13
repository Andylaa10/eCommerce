using AutoMapper;
using UserService.Core.Entities;
using UserService.Core.Services.DTOs;

namespace UserService.Configs;

public static class AutoMapperConfig
{
    public static IMapper ConfigureAutoMapper()
    {
        var mapper = new MapperConfiguration(options =>
        {
            // DTO to Entity
            options.CreateMap<CreateUserDto, User>();
            options.CreateMap<UpdateUserDto, User>();
            
            // Entity to DTO
            options.CreateMap<User, GetUserDto>();

        }).CreateMapper();

        return mapper;
    }
}