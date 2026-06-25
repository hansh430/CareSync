using CareSync.Common;
using CareSync.Dtos;

namespace CareSync.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<UserDto>> RegisterAsync(RegisterDto model);
        Task<ServiceResponse<LoginResponseDto>> LoginAsync(LoginDto model);
        Task<ServiceResponse<UserDto>> GetUserByIdAsync(int id);
        Task<ServiceResponse<UserDto>> UpdateUserAsync(int id, UpdateUserDto model);
        Task<ServiceResponse<UserHomeDto>> GetHomeDataAsync(int userId);
    }
}
