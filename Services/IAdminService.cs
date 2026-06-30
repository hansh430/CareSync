using CareSync.Common;
using CareSync.Dtos;
using CareSync.Models;

namespace CareSync.Services
{
    public interface IAdminService
    {
        Task<ServiceResponse<LoginResponseDto>> AdminLoginAsync(AdminLoginDto model);
        Task<ServiceResponse<DashboardDto>> GetDashboardAsync();
        Task<ServiceResponse<OrderDetailsDto>> GetOrderDetailsAsync(int orderId);
        Task<ServiceResponse<List<UserDto>>> GetUsersAsync();
        Task<ServiceResponse<User>> AddFundAsync(int userId, FundDto model);
        Task<ServiceResponse<PagedResultDto<OrderDto>>> GetOrdersAsync(int page, int pageSize);
        Task<ServiceResponse<Order>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto model);
    }
}
