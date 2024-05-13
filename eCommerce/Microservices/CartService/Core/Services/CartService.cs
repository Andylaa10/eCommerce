using AutoMapper;
using Cache;
using CartService.Core.Entities;
using CartService.Core.Repositories.Interfaces;
using CartService.Core.Services.DTOs;
using CartService.Core.Services.Interfaces;

namespace CartService.Core.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly RedisClient _redisClient;


    public CartService(ICartRepository cartRepository, IMapper mapper, RedisClient redisClient)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _redisClient = redisClient;
        _redisClient.Connect();
    }

    public async Task<Cart> CreateCart(CreateCartDto dto)
    {
        var cart = await _cartRepository.CreateCart(_mapper.Map<Cart>(dto));

        var productJson = _redisClient.SerializeObject(cart);
        _redisClient.StoreValue(cart.Id.ToString(), productJson);

        return cart;
    }

    public async Task<Cart> GetCartByUserId(int userId)
    {
        if (userId <= 0)
            throw new ArgumentException("UserId cannot be 0");

        var cartJson = _redisClient.GetValue(userId.ToString());

        if (!string.IsNullOrEmpty(cartJson))
            return await Task.FromResult(_redisClient.DeserializeObject<Cart>(cartJson)!);

        return await _cartRepository.GetCartByUserId(userId);
    }

    public async Task<Cart> AddProductToCart(int cartId, AddProductToCartDto dto)
    {
        if (cartId <= 0)
            throw new ArgumentException("UserId cannot be 0");

        var cart = await _cartRepository.AddProductToCart(cartId, _mapper.Map<ProductLine>(dto));

        var cartJson = _redisClient.SerializeObject(cart);
        _redisClient.StoreValue(cart.Id.ToString(), cartJson);

        return cart;
    }

    public async Task<Cart> RemoveProductFromCart(int cartId, string productId)
    {
        if (cartId <= 0)
            throw new ArgumentException("UserId cannot be 0");
        if (string.IsNullOrEmpty(productId))
            throw new ArgumentException("ProductId cannot be null or empty");

        var cart = await _cartRepository.RemoveProductFromCart(cartId, productId);
        _redisClient.RemoveValue(productId);

        return cart;
    }

    public async Task RebuildDatabase()
    {
        await _cartRepository.RebuildDatabase();
    }
}