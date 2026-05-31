using CareSync.Common;
using CareSync.Dtos;

namespace CareSync.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<UserDto>> RegisterAsync(RegisterDto model);
        Task<ServiceResponse<UserDto>> LoginAsync(LoginDto model);
    }
}
