using CartService.Core.Entities;
using CartService.Core.Services.DTOs;

namespace CartService.Core.Services.Interfaces;

public interface ICartService
{
    public Task<Cart> CreateCart(CreateCartDto dto);
    public Task<Cart> GetCartByUserId(int userId);
    public Task<Cart> UpdateCart(int userId, UpdateCartDto cart);
    public Task<Cart> AddProductToCart(int userId, AddProductToCartDto dto);
    public Task<Cart> RemoveProductFromCart(int userId, string productId);
    public Task<Cart> DeleteCart(int cartId);

}