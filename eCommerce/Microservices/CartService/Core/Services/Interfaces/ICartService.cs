using CartService.Core.Entities;
using CartService.Core.Services.DTOs;

namespace CartService.Core.Services.Interfaces;

public interface ICartService
{
    public Task<Cart> CreateCart(CreateCartDto dto);
    public Task<Cart> GetCartByUserId(int userId);

    public Task<Cart> AddProductToCart(int cartId, AddProductToCartDto dto);
    public Task<Cart> RemoveProductFromCart(int cartId, string productId);
}