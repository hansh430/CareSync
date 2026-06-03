using CareSync.Common;
using CareSync.Data;
using CareSync.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Services
{
    public class AdminService : IAdminService
    {
        private readonly EmedicineContext _context;

        public AdminService(EmedicineContext context)
        {
            _context = context;
        }
        public async Task<ServiceResponse<UserDto>> AdminLoginAsync(AdminLoginDto model)
        {
            var response = new ServiceResponse<UserDto>();
            var admin = await _context.Users
                       .FirstOrDefaultAsync(x =>
                        x.Email == model.Email &&
                         x.Type == "Admin");

            if (admin == null)
            {
                response.Success = false;
                response.Message = "Invalid admin credentials.";
                return response;
            }
            bool validPassword = BCrypt.Net.BCrypt.Verify(model.Password, admin.Password);
            if (!validPassword)
            {
                response.Success = false;
                response.Message = "Invalid admin credentials.";
                return response;
            }
            response.Data = new UserDto
            {
                Id = admin.Id,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Email = admin.Email,
            };
            response.Message = "Admin login successful";
            return response;
        }

        public async Task<ServiceResponse<DashboardDto>> GetDashboardAsync()
        {
            var response = new ServiceResponse<DashboardDto>();
            response.Data = new DashboardDto
            {
                TotalCustomers = await _context.Users
               .CountAsync(x => x.Type == "User"),

                TotalMedicines = await _context.Medicines
               .CountAsync(),

                TotalOrders = await _context.Orders
               .CountAsync(),

                TotalRevenue = await _context.Orders
               .SumAsync(x => x.OrderTotal ?? 0)
            };

            return response;
        }
    }
}
