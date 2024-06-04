using System.Data;
using AutoMapper;
using Cache;
using Messaging;
using Messaging.SharedMessages;
using MonitoringService;
using OpenTelemetry.Trace;
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
    private readonly Tracer _tracer;

    public UserService(IUserRepository userRepository, IMapper mapper, IRedisClient redisClient,
        MessageClient messageClient, Tracer tracer)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _redisClient = redisClient;
        _messageClient = messageClient;
        _tracer = tracer;
        _redisClient.Connect();
    }


    public async Task<GetUserDto> GetUserById(int id)
    {
        using var activity = _tracer.StartActiveSpan("GetUserById");

        if (id < 1)
            throw new ArgumentException("Id could be less than 0");

        try
        {
            LoggingService.Log.Information("Called GetUserById Method");

            var userJson = await _redisClient.GetValue($"User:{id}");

            if (!string.IsNullOrEmpty(userJson))
                return await Task.FromResult(_redisClient.DeserializeObject<GetUserDto>(userJson)!);

            var user = _mapper.Map<GetUserDto>(await _userRepository.GetUserById(id));

            return user;
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            throw new ArgumentException(e.Message);
        }
    }

    public async Task<IEnumerable<GetUserDto>> GetAllUsers()
    {
        using var activity = _tracer.StartActiveSpan("GetAllUsers");

        try
        {
            LoggingService.Log.Information("Called GetAllUsers Method");

            var users = await _userRepository.GetAllUsers();
            return _mapper.Map<IEnumerable<GetUserDto>>(users);
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            throw new ArgumentException(e.Message);
        }
    }

    public async Task<GetUserDto> AddUser(CreateUserDto dto)
    {
        using var activity = _tracer.StartActiveSpan("AddUser");

        var exists = await _userRepository.DoesUserExist(dto.Email);

        if (exists)
            throw new DuplicateNameException($"{dto.Email} is already in use");

        try
        {
            LoggingService.Log.Information("Called AddUser Method");

            var user = _mapper.Map<GetUserDto>(await _userRepository.AddUser(_mapper.Map<User>(dto)));

            var userJson = _redisClient.SerializeObject(user);
            await _redisClient.StoreValue($"User:{user.Id}", userJson);

            const string exchangeName = "CreateCartExchange";
            const string routingKey = "CreateCart";

            _messageClient.Send(new CreateCartMessage("Create Cart", user.Id, 0.0f), exchangeName, routingKey);

            return user;
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            throw new ArgumentException(e.Message);
        }
    }

    public async Task<GetUserDto> UpdateUser(int id, UpdateUserDto dto)
    {
        using var activity = _tracer.StartActiveSpan("UpdateUser");

        if (id < 1)
            throw new ArgumentException("Id could be less than 0");

        if (id != dto.Id)
            throw new ArgumentException("Id int the route does not match the id of the user");

        try
        {
            LoggingService.Log.Information("Called UpdateUser Method");

            var user = _mapper.Map<GetUserDto>(await _userRepository.UpdateUser(id, _mapper.Map<User>(dto)));

            var userJson = _redisClient.SerializeObject(user);
            await _redisClient.StoreValue($"User:{id}", userJson);

            return user;
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            throw new ArgumentException(e.Message);
        }
    }

    public async Task<GetUserDto> DeleteUser(int id)
    {
        using var activity = _tracer.StartActiveSpan("DeleteUser");

        if (id < 1)
            throw new ArgumentException("Id could be less than 0");

        try
        {
            LoggingService.Log.Information("Called DeleteUser Method");

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
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            throw new ArgumentException(e.Message);
        }
    }

    public async Task RebuildDatabase()
    {
        using var activity = _tracer.StartActiveSpan("Rebuild User Database");

        try
        {
            LoggingService.Log.Information("Called Rebuild User Database Method");

            await _userRepository.RebuildDatabase();
        }
        catch (Exception e)
        {
            LoggingService.Log.Error(e.Message);
            throw new ArgumentException(e.Message);
        }
    }
}