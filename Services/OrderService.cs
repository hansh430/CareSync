using CareSync.Common;
using CareSync.Data;
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
        public async Task<ServiceResponse<Order>> PlaceOrderAsync(int userId)
        {
            var response = new ServiceResponse<Order>();

            var cartItems = await _context.Carts
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                response.Success = false;
                response.Message = "Cart is empty.";
                return response;
            }
            decimal orderTotal = cartItems.Sum(c => c.TotalPrice ?? 0);
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

            response.Data = order;
            response.Message = "Order placed successfully.";

            return response;
        }
    }
}
