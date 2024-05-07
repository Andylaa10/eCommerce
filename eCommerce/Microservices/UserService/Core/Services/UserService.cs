using System.Data;
using AutoMapper;
using UserService.Core.Entities;
using UserService.Core.Repositories.Interfaces;
using UserService.Core.Services.DTOs;
using UserService.Core.Services.Interfaces;

namespace UserService.Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }


    public async Task<GetUserDto> GetUserById(int id)
    {
        if (id < 1)
            throw new ArgumentException("Id could be less than 0");

        return _mapper.Map<GetUserDto>(await _userRepository.GetUserById(id));
    }

    public async Task<IEnumerable<GetUserDto>> GetAllUsers()
    {
        var users = await _userRepository.GetAllUsers();
        return _mapper.Map<IEnumerable<GetUserDto>>(users);
    }

    public async Task AddUser(CreateUserDto user)
    {
        var exists = await _userRepository.DoesUserExist(user.Email);

        if (exists)
            throw new DuplicateNameException($"{user.Email} is already in use");

        await _userRepository.AddUser(_mapper.Map<User>(user));
    }

    public async Task UpdateUser(int id, UpdateUserDto user)
    {
        if (id < 1)
            throw new ArgumentException("Id could be less than 0");

        if (id != user.Id)
            throw new ArgumentException("Id int the route does not match the id of the user");

        await _userRepository.UpdateUser(id, _mapper.Map<User>(user));
    }

    public async Task DeleteUser(int id)
    {
        if (id < 1)
            throw new ArgumentException("Id could be less than 0");

        await _userRepository.DeleteUser(id);
    }
}