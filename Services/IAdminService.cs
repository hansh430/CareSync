using CareSync.Common;
using CareSync.Dtos;
using CareSync.Models;

namespace CareSync.Services
{
    public interface IAdminService
    {
        Task<ServiceResponse<UserDto>> AdminLoginAsync(AdminLoginDto model);
        Task<ServiceResponse<DashboardDto>> GetDashboardAsync();
        Task<ServiceResponse<List<UserDto>>> GetUsersAsync();
        Task<ServiceResponse<User>> AddFundAsync(int userId, FundDto model);
        Task<ServiceResponse<List<OrderDto>>> GetOrdersAsync();
        Task<ServiceResponse<Order>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto model);
    }
}
