using System.Data;
using AutoMapper;
using Cache;
using UserService.Core.Entities;
using UserService.Core.Repositories.Interfaces;
using UserService.Core.Services.DTOs;
using UserService.Core.Services.Interfaces;

namespace UserService.Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly RedisClient _redisClient;

    public UserService(IUserRepository userRepository, IMapper mapper, RedisClient redisClient)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _redisClient = redisClient;
        _redisClient.Connect();
    }


    public async Task<GetUserDto> GetUserById(int id)
    {
        if (id < 1)
            throw new ArgumentException("Id could be less than 0");
        
        var userJson = _redisClient.GetValue(id.ToString());

        if (!string.IsNullOrEmpty(userJson))
            return await Task.FromResult(_redisClient.DeserializeObject<GetUserDto>(userJson)!);

        var user = _mapper.Map<GetUserDto>(await _userRepository.GetUserById(id));
        
        return user;
    }

    public async Task<IEnumerable<GetUserDto>> GetAllUsers()
    {
        var users = await _userRepository.GetAllUsers();
        return _mapper.Map<IEnumerable<GetUserDto>>(users);
        
        //Find a good way to cache all users, how should I update the cache 
    }

    public async Task<GetUserDto> AddUser(CreateUserDto dto)
    {
        var exists = await _userRepository.DoesUserExist(dto.Email);

        if (exists)
            throw new DuplicateNameException($"{dto.Email} is already in use");

        var user = _mapper.Map<GetUserDto>(await _userRepository.AddUser(_mapper.Map<User>(dto)));
        
        var userJson = _redisClient.SerializeObject(user);
        _redisClient.StoreValue(user.Id.ToString(), userJson);
        
        return user;
    }

    public async Task<GetUserDto> UpdateUser(int id, UpdateUserDto dto)
    {
        if (id < 1)
            throw new ArgumentException("Id could be less than 0");

        if (id != dto.Id)
            throw new ArgumentException("Id int the route does not match the id of the user");

        var user = _mapper.Map<GetUserDto>(await _userRepository.UpdateUser(id, _mapper.Map<User>(dto)));
        
        var userJson = _redisClient.SerializeObject(user);
        _redisClient.StoreValue(id.ToString(), userJson);
        
        return user;
    }

    public async Task<GetUserDto> DeleteUser(int id)
    {
        if (id < 1)
            throw new ArgumentException("Id could be less than 0");

        var user = _mapper.Map<GetUserDto>(await _userRepository.DeleteUser(id));
        
        _redisClient.RemoveValue(id.ToString());
        
        return user;
    }
    
    public async Task RebuildDatabase()
    {
        await _userRepository.RebuildDatabase();
    }
}