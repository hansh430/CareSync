using CareSync.Common;
using CareSync.Dtos;
using CareSync.Models;

namespace CareSync.Services
{
    public interface IOrderService
    {
        Task<ServiceResponse<Order>> PlaceOrderAsync(int userId);
        Task<ServiceResponse<List<OrderDto>>> GetOrderByUserAsync(int userId);
        Task<ServiceResponse<List<OrderItemDto>>> GetOrderDetailsAsync(int orderId);
    }
}
