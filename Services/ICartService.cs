using CareSync.Common;
using CareSync.Dtos;
using CareSync.Models;

namespace CareSync.Services
{
    public interface ICartService
    {
        Task<ServiceResponse<Cart>> AddToCartAsync(AddCartDto model);
    }
}