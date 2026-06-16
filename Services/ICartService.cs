using CareSync.Common;
using CareSync.Dtos;
using CareSync.Models;

namespace CareSync.Services
{
    public interface ICartService
    {
        Task<ServiceResponse<Cart>> AddToCartAsync(int userId, AddCartDto model);
        Task<ServiceResponse<List<CartItemDto>>> GetCartByUserAsync(int userId);
        Task<ServiceResponse<Cart>> UpdateCartAsync(int cartId, UpdateCartDto model);
        Task<ServiceResponse<bool>> RemoveCartItemAsync(int cartId);
    }
}