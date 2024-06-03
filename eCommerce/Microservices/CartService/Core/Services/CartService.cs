using System.Diagnostics;
using AutoMapper;
using Cache;
using CartService.Core.Entities;
using CartService.Core.Repositories.Interfaces;
using CartService.Core.Services.DTOs;
using CartService.Core.Services.Interfaces;
using Messaging;
using Messaging.SharedMessages;
using MonitoringService;
using OpenTelemetry.Trace;
using Polly;
using Polly.CircuitBreaker;

namespace CartService.Core.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly IRedisClient _redisClient;
    private readonly MessageClient _messageClient;
    private readonly Tracer _tracer;


    public CartService(ICartRepository cartRepository, IMapper mapper, IRedisClient redisClient,
        MessageClient messageClient, Tracer tracer)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _redisClient = redisClient;
        _messageClient = messageClient;
        _redisClient.Connect();
        _tracer = tracer;
    }

    public async Task<Cart> CreateCart(CreateCartDto dto)
    {
        try
        {
            // Retry policy
            var retryPolicy = Policy<Cart>
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // exponential back-off
                    (exception, timeSpan, retryCount, context) =>
                    {
                        var retryMessage =
                            $"Retry {retryCount} : {exception.Exception.Message}. Waiting {timeSpan} before next retry.";

                        LoggingService.Log.Information(retryMessage);
                        Console.WriteLine(retryMessage);
                    });

            //Fallback Policy  
            var fallbackPolicy = Policy<Cart>
                .Handle<Exception>()
                .FallbackAsync((context) =>
                {
                    var cart = new Cart();
                    LoggingService.Log.Warning("Called Fallback Method in CartService " + context);
                    return Task.FromResult(cart);
                });


            return await fallbackPolicy.WrapAsync(retryPolicy)
                .ExecuteAsync(async () =>
                {
                    _tracer.StartActiveSpan("CreateCart");
                    var cart = await _cartRepository.CreateCart(_mapper.Map<Cart>(dto));
                    var cartJson = _redisClient.SerializeObject(cart);
                    await _redisClient.StoreValue($"Cart:{dto.UserId}", cartJson);
                    return cart;
                });
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<Cart> GetCartByUserId(int userId)
    {
        _tracer.StartActiveSpan("GetCartByUserId");

        if (userId < 1)
            throw new ArgumentException("UserId cannot be less than 1");

        var cartJson = await _redisClient.GetValue($"Cart:{userId}");

        if (!string.IsNullOrEmpty(cartJson))
            return await Task.FromResult(_redisClient.DeserializeObject<Cart>(cartJson)!);

        return await _cartRepository.GetCartByUserId(userId);
    }

    public async Task<Cart> UpdateCart(int userId, UpdateCartDto dto)
    {
        _tracer.StartActiveSpan("UpdateCart");

        if (userId < 1)
            throw new ArgumentException("UserId can not be less than 1");

        var cart = await _cartRepository.UpdateCart(userId, _mapper.Map<Cart>(dto));

        var cartJson = _redisClient.SerializeObject(cart);
        await _redisClient.StoreValue($"Cart:{userId}", cartJson);

        return cart;
    }

    public async Task<Cart> AddProductToCart(int userId, AddProductToCartDto dto)
    {
        _tracer.StartActiveSpan("AddProductToCart");

        if (userId < 1)
            throw new ArgumentException("UserId can not be less than 1");

        var cart = await _cartRepository.AddProductToCart(userId, _mapper.Map<ProductLine>(dto));

        var cartJson = _redisClient.SerializeObject(cart);
        await _redisClient.StoreValue($"Cart:{userId}", cartJson);

        const string exchangeName = "UpdateCartExchange";
        const string routingKey = "UpdateCart";
        _messageClient.Send(new UpdateCartMessage("Update cart", userId), exchangeName, routingKey);

        return cart;
    }

    public async Task<Cart> RemoveProductFromCart(int userId, string productId)
    {
        _tracer.StartActiveSpan("RemoveProductFromCart");


        if (userId < 1)
            throw new ArgumentException("UserId can not be less than 1");

        if (string.IsNullOrEmpty(productId))
            throw new ArgumentException("ProductId cannot be null or empty");

        var cart = await _cartRepository.RemoveProductFromCart(userId, productId);

        var cartJson = _redisClient.SerializeObject(cart);
        await _redisClient.StoreValue($"Cart:{userId}", cartJson);

        const string exchangeName = "UpdateCartExchange";
        const string routingKey = "UpdateCart";
        _messageClient.Send(new UpdateCartMessage("Update cart", userId), exchangeName, routingKey);

        return cart;
    }

    public async Task<Cart> DeleteCart(int userId)
    {
        _tracer.StartActiveSpan("DeleteCart");

        if (userId < 1)
            throw new ArgumentException("UserId can not be less than 1");

        await _redisClient.RemoveValue($"Cart:{userId}");

        return await _cartRepository.DeleteCart(userId);
    }
}