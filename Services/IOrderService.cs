using CareSync.Common;
using CareSync.Models;

namespace CareSync.Services
{
    public interface IOrderService
    {
        Task<ServiceResponse<Order>> PlaceOrderAsync(int userId);
    }
}
