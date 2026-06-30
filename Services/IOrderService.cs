using CareSync.Common;
using CareSync.Dtos;
using CareSync.Models;

namespace CareSync.Services
{
    public interface IOrderService
    {
        Task<ServiceResponse<Order>> PlaceOrderAsync(int userId);
        Task<ServiceResponse<PagedResultDto<OrderDto>>> GetOrderByUserAsync(int userId, int page, int pageSize);
        Task<ServiceResponse<List<OrderItemDto>>> GetOrderDetailsAsync(int orderId);
    }
}
