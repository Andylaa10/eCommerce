using AutoMapper;
using Cache;
using CartService.Core.Entities;
using CartService.Core.Repositories.Interfaces;
using CartService.Core.Services.DTOs;
using CartService.Core.Services.Interfaces;
using Messaging;
using Messaging.SharedMessages;

namespace CartService.Core.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly RedisClient _redisClient;
    private readonly MessageClient _messageClient;


    public CartService(ICartRepository cartRepository, IMapper mapper, RedisClient redisClient,
        MessageClient messageClient)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _redisClient = redisClient;
        _messageClient = messageClient;
        _redisClient.Connect();
    }

    public async Task<Cart> CreateCart(CreateCartDto dto)
    {
        var cart = await _cartRepository.CreateCart(_mapper.Map<Cart>(dto));

        var cartJson = _redisClient.SerializeObject(cart);
        await _redisClient.StoreValue($"Cart:{dto.UserId}", cartJson);

        return cart;
    }

    public async Task<Cart> GetCartByUserId(int userId)
    {
        //TODO Caching is not working for cart by is working for the other services
        if (userId < 1)
            throw new ArgumentException("UserId cannot be less than 1");
        
        var cartJson = await _redisClient.GetValue($"Cart:{userId}");
        
        if (!string.IsNullOrEmpty(cartJson))
            return await Task.FromResult(_redisClient.DeserializeObject<Cart>(cartJson)!);
        
        return await _cartRepository.GetCartByUserId(userId);
    }

    public async Task<Cart> UpdateCart(int userId, UpdateCartDto dto)
    {
        if (userId < 1)
            throw new ArgumentException("UserId can not be less than 1");

        var cart = await _cartRepository.UpdateCart(userId, _mapper.Map<Cart>(dto));

        var cartJson = _redisClient.SerializeObject(cart);
        await _redisClient.StoreValue($"Cart:{userId}", cartJson);

        return cart;
    }
    
    public async Task<Cart> AddProductToCart(int userId, AddProductToCartDto dto)
    {
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
        if (userId < 1)
            throw new ArgumentException("UserId can not be less than 1");

        await _redisClient.RemoveValue($"Cart:{userId}");

        return await _cartRepository.DeleteCart(userId);
    }
}