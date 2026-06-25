using CareSync.Common;
using CareSync.Data;
using CareSync.Dtos;
using CareSync.Models;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Services
{
    public class OrderService : IOrderService
    {
        private readonly EmedicineContext _context;

        public OrderService(EmedicineContext context)
        {
            _context = context;
        }

        //--------------------place order  -------------------------------------

        public async Task<ServiceResponse<Order>> PlaceOrderAsync(int userId)
        {
            var response = new ServiceResponse<Order>();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }
                var cartItems = await _context.Carts
                     .Where(c => c.UserId == userId)
                     .ToListAsync();
                if (!cartItems.Any())
                {
                    response.Success = false;
                    response.Message = "Cart is Empty";
                    return response;
                }

                decimal orderTotal = cartItems.Sum(c => c.TotalPrice ?? 0);

                //Check Wallet balance ---

                if ((user.Fund ?? 0) < orderTotal)
                {
                    response.Success = false;
                    response.Message = "Insuffiencient wallet balacne.";
                    return response;
                }

                // Deduct amount from wallet
                user.Fund = (user.Fund ?? 0) - orderTotal;

                var order = new Order
                {
                    UserId = userId,
                    OrderNo = $"ORD-{DateTime.Now:yyyyMMddHHmmss}",
                    OrderTotal = orderTotal,
                    OrderStatus = "Pending"
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                foreach (var item in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        MedicineId = item.MedicineId,
                        UnitPrice = item.UnitPrice,
                        Discount = item.Discount,
                        Quantity = item.Quantity,
                        TotalPrice = item.TotalPrice
                    };

                    _context.OrderItems.Add(orderItem);
                }
                _context.Carts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                response.Data = order;
                response.Message = "Order placed successfully.";
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }

        }

        //--------------------Get list of orders for a user -------------------------------------

        public async Task<ServiceResponse<List<OrderDto>>> GetOrderByUserAsync(int userId)
        {
            var response = new ServiceResponse<List<OrderDto>>();

            response.Data = await _context.Orders.Where(o => o.UserId == userId)
                .OrderByDescending(o => o.Id)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderNo = o.OrderNo,
                    OrderTotal = o.OrderTotal,
                    OrderStatus = o.OrderStatus,
                }).ToListAsync();

            return response;
        }

        //--------------------Get order details -------------------------------------
        public async Task<ServiceResponse<List<OrderItemDto>>> GetOrderDetailsAsync(int orderId)
        {
            var response = new ServiceResponse<List<OrderItemDto>>();

            response.Data = await (
                from oi in _context.OrderItems
                join m in _context.Medicines
                on oi.MedicineId equals m.Id
                where oi.OrderId == orderId
                select new OrderItemDto
                {
                    MedicineId = m.Id,
                    MedicineName = m.Name,
                    ImageUrl = m.ImageUrl,
                    UnitPrice = oi.UnitPrice,
                    Discount = oi.Discount,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.TotalPrice
                }).ToListAsync();
            return response;
        }

    }
}
