using CareSync.Common;
using CareSync.Data;
using CareSync.Dtos;
using CareSync.Models;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Services
{
    public class AdminService : IAdminService
    {
        private readonly EmedicineContext _context;
        private readonly ITokenService _tokenService;
        public AdminService(EmedicineContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }


        public async Task<ServiceResponse<LoginResponseDto>> AdminLoginAsync(AdminLoginDto model)
        {
            var response = new ServiceResponse<LoginResponseDto>();

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

            bool validPassword =
                BCrypt.Net.BCrypt.Verify(
                    model.Password,
                    admin.Password);

            if (!validPassword)
            {
                response.Success = false;
                response.Message = "Invalid admin credentials.";

                return response;
            }

            var token = _tokenService.CreateToken(admin);

            response.Data = new LoginResponseDto
            {
                Token = token,

                User = new UserDto
                {
                    Id = admin.Id,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    Email = admin.Email,
                    Fund = admin.Fund,
                    Type = admin.Type,
                    Status = admin.Status
                }
            };

            response.Message = "Admin login successful.";

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

        //-------------------- Get All Users --------------------------------------------//
        public async Task<ServiceResponse<List<UserDto>>> GetUsersAsync()
        {
            var response = new ServiceResponse<List<UserDto>>();

            response.Data = await _context.Users.Where(x => x.Type == "User")
                               .Select(x => new UserDto
                               {
                                   Id = x.Id,
                                   FirstName = x.FirstName,
                                   LastName = x.LastName,
                                   Email = x.Email,
                                   Fund = x.Fund,
                                   Type = x.Type,
                                   Status = x.Status
                               })
                                 .ToListAsync();

            response.Message = "Users retrieved successfully.";
            return response;
        }

        //-------------------- Add Fund to user --------------------------------------------//
        public async Task<ServiceResponse<User>> AddFundAsync(int userId, FundDto model)
        {
            var response = new ServiceResponse<User>();

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId && x.Type == "User");
            if (user == null)
            {
                response.Success = false;
                response.Message = "Customer not found.";
                return response;
            }
            user.Fund = (user.Fund ?? 0) + model.Amount;
            await _context.SaveChangesAsync();
            response.Data = user;
            response.Message = "Fund added successfully.";
            return response;
        }

        //-------------------- Fetch all orders --------------------------------------------//
        public async Task<ServiceResponse<List<OrderDto>>> GetOrdersAsync()
        {
            var response = new ServiceResponse<List<OrderDto>>();

            response.Data = await _context.Orders
                .Select(x => new OrderDto
                {
                    Id = x.Id,
                    OrderNo = x.OrderNo,
                    OrderTotal = x.OrderTotal,
                    OrderStatus = x.OrderStatus
                })
                .ToListAsync();

            return response;
        }

        //-------------------- Update order status --------------------------------------------//
        public async Task<ServiceResponse<Order>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto model)
        {
            var response = new ServiceResponse<Order>();

            var order = await _context.Orders
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
            {
                response.Success = false;
                response.Message = "Order not found";
                return response;
            }

            order.OrderStatus = model.Status;

            await _context.SaveChangesAsync();

            response.Data = order;
            response.Message = "Status updated successfully";

            return response;
        }
    }
}
