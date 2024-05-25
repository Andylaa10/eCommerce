﻿using System.Data;
using AutoMapper;
using Cache;
using Messaging;
using Messaging.SharedMessages;
using UserService.Core.Entities;
using UserService.Core.Repositories.Interfaces;
using UserService.Core.Services.DTOs;
using UserService.Core.Services.Interfaces;

namespace UserService.Core.Services;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly MessageClient _messageClient;
    private readonly IRedisClient _redisClient;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, IMapper mapper, IRedisClient redisClient,
        MessageClient messageClient)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _redisClient = redisClient;
        _messageClient = messageClient;
        _redisClient.Connect();
    }


    public async Task<GetUserDto> GetUserById(int id)
    {
        if (id < 1)
            throw new ArgumentException("Id could be less than 0");

        var userJson = await _redisClient.GetValue($"User:{id}");

        if (!string.IsNullOrEmpty(userJson))
            return await Task.FromResult(_redisClient.DeserializeObject<GetUserDto>(userJson)!);

        var user = _mapper.Map<GetUserDto>(await _userRepository.GetUserById(id));

        return user;
    }

    public async Task<IEnumerable<GetUserDto>> GetAllUsers()
    {
        var users = await _userRepository.GetAllUsers();
        return _mapper.Map<IEnumerable<GetUserDto>>(users);
    }

    public async Task<GetUserDto> AddUser(CreateUserDto dto)
    {
        var exists = await _userRepository.DoesUserExist(dto.Email);

        if (exists)
            throw new DuplicateNameException($"{dto.Email} is already in use");

        var user = _mapper.Map<GetUserDto>(await _userRepository.AddUser(_mapper.Map<User>(dto)));

        var userJson = _redisClient.SerializeObject(user);
        await _redisClient.StoreValue($"User:{user.Id}", userJson);

        const string exchangeName = "CreateCartExchange";
        const string routingKey = "CreateCart";


        _messageClient.Send(new CreateCartMessage("Create Cart", user.Id, 0.0f), exchangeName, routingKey);

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
        await _redisClient.StoreValue($"User:{id}", userJson);

        return user;
    }

    public async Task<GetUserDto> DeleteUser(int id)
    {
        if (id < 1)
            throw new ArgumentException("Id could be less than 0");

        var user = _mapper.Map<GetUserDto>(await _userRepository.DeleteUser(id));

        await _redisClient.RemoveValue($"User:{id}");

        const string exchangeNameCart = "DeleteCartExchange";
        const string routingKeyCart = "DeleteCart";

        _messageClient.Send(new DeleteCartMessage("Delete Cart", user.Id), exchangeNameCart, routingKeyCart);

        const string exchangeNameAuth = "DeleteAuthExchange";
        const string routingKeyAuth = "DeleteAuth";
        
        _messageClient.Send(new DeleteAuthMessage("Delete Auth", id), exchangeNameAuth, routingKeyAuth);

        return user;
    }

    public async Task RebuildDatabase()
    {
        await _userRepository.RebuildDatabase();
    }
}