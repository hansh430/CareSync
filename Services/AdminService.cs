using Azure;
using CareSync.Common;
using CareSync.Data;
using CareSync.Dtos;
using CareSync.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
                .Where(x => x.OrderStatus == "Done")
               .SumAsync(x => x.OrderTotal ?? 0),

                PendingOrders = await _context.Orders.CountAsync(x => x.OrderStatus == "Pending")
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
        public async Task<ServiceResponse<PagedResultDto<OrderDto>>> GetOrdersAsync(int page, int pageSize)
        {
            var response = new ServiceResponse<PagedResultDto<OrderDto>>();

            var query = from order in _context.Orders
                        join user in _context.Users
                        on order.UserId equals user.Id
                        orderby order.Id descending
                        select new OrderDto
                        {
                            Id = order.Id,
                            OrderNo = order.OrderNo,
                            UserName = user.FirstName + " " + user.LastName,
                            OrderTotal = order.OrderTotal,
                            OrderStatus = order.OrderStatus
                        };

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            response.Data = new PagedResultDto<OrderDto>
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return response;
        }

        //-------------------- Get Order details --------------------------------------------//
        public async Task<ServiceResponse<OrderDetailsDto>> GetOrderDetailsAsync(int orderId)
        {
            var response = new ServiceResponse<OrderDetailsDto>();
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            if (order == null)
            {
                response.Success = false;
                response.Message = "Order not found";
                return response;
            }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == order.UserId);
            var items = await (
                from orderItem in _context.OrderItems
                join medicine in _context.Medicines
                on orderItem.MedicineId equals medicine.Id
                where orderItem.OrderId == orderId
                select new OrderItemDto
                {
                    MedicineId = medicine.Id,

                    MedicineName = medicine.Name,

                    ImageUrl = medicine.ImageUrl,

                    UnitPrice = orderItem.UnitPrice,

                    Discount = orderItem.Discount,

                    Quantity = orderItem.Quantity,

                    TotalPrice = orderItem.TotalPrice
                }).ToListAsync();
            response.Data =
        new OrderDetailsDto
        {
            Id = order.Id,

            OrderNo = order.OrderNo,

            UserName = user == null
                    ? ""
                    : user.FirstName + " " + user.LastName,

            OrderTotal = order.OrderTotal,

            OrderStatus = order.OrderStatus,

            Items = items
        };

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

            string currentStatus = order.OrderStatus ?? "";
            string newStatus = model.Status;

            var error = GetInvalidTransitionMessage(currentStatus, newStatus);

            if (error != null)
            {
                response.Success = false;
                response.Message = error;
                return response;
            }

            // Refund amount and restore stock when cancelled
            await RefundIfOrderCanceled(newStatus, order);

            order.OrderStatus = newStatus;

            await _context.SaveChangesAsync();

            response.Data = order;
            response.Message = "Status updated successfully";

            return response;

        }

        private async Task RefundIfOrderCanceled(string newStatus, Order order)
        {
            if (newStatus == "Cancelled")
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == order.UserId);
                if (user != null)
                {
                    user.Fund = (user.Fund ?? 0) + (order.OrderTotal ?? 0);
                }
                var orderItems = await _context.OrderItems
                    .Where(x => x.OrderId == order.Id).ToListAsync();

                foreach (var item in orderItems)
                {
                    var medicine = await _context.Medicines.FirstOrDefaultAsync(x => x.Id == item.MedicineId);
                    if (medicine != null)
                    {
                        medicine.Quantity += item.Quantity ?? 0;
                    }
                }
            }
        }
        private string? GetInvalidTransitionMessage(string currentStatus, string newStatus)
        {
            switch (currentStatus)
            {
                case "Done":
                    return "Completed orders cannot be modified.";

                case "Cancelled":
                    return "Cancelled orders cannot be modified.";

                case "Pending":
                    if (newStatus != "Processing" &&
                        newStatus != "Cancelled")
                    {
                        return "Pending order can only move to Processing or Cancelled.";
                    }
                    break;

                case "Processing":
                    if (newStatus != "Done" &&
                        newStatus != "Cancelled")
                    {
                        return "Processing order can only move to Done or Cancelled.";
                    }
                    break;
            }

            return null;
        }

    }

}
