using CareSync.Common;
using CareSync.Dtos;

namespace CareSync.Services
{
    public interface IAdminService
    {
        Task<ServiceResponse<UserDto>> AdminLoginAsync(AdminLoginDto model);
        Task<ServiceResponse<DashboardDto>> GetDashboardAsync();
    }
}
