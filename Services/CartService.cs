using CareSync.Common;
using CareSync.Data;
using CareSync.Dtos;
using CareSync.Models;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Services
{
    public class CartService : ICartService
    {
        private readonly EmedicineContext _context;

        public CartService(EmedicineContext context)
        {
            _context = context;
        }

        //-------------------- Add To cart --------------------------------------------//
        public async Task<ServiceResponse<Cart>> AddToCartAsync(int userId, AddCartDto model)
        {
            var response = new ServiceResponse<Cart>();

            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.Id == model.MedicineId);

            if (medicine == null)
            {
                response.Success = false;
                response.Message = "Medicine not found.";
                return response;
            }

            decimal unitPrice = medicine.UnitPrice ?? 0;
            decimal discount = medicine.Discount ?? 0;

            // Price after discount
            decimal finalPrice = unitPrice - discount;

            // Check if medicine already exists in cart
            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c =>
                    c.UserId == userId &&
                    c.MedicineId == model.MedicineId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity =
                    (existingCartItem.Quantity ?? 0) + model.Quantity;

                existingCartItem.UnitPrice = unitPrice;
                existingCartItem.Discount = discount;

                existingCartItem.TotalPrice =
                    finalPrice * (existingCartItem.Quantity ?? 0);

                await _context.SaveChangesAsync();

                response.Data = existingCartItem;
                response.Message = "Cart updated successfully.";

                return response;
            }

            var cart = new Cart
            {
                UserId = userId,
                MedicineId = model.MedicineId,
                Quantity = model.Quantity,
                UnitPrice = unitPrice,
                Discount = discount,
                TotalPrice = finalPrice * model.Quantity
            };

            _context.Carts.Add(cart);

            await _context.SaveChangesAsync();

            response.Data = cart;
            response.Message = "Added to cart.";

            return response;
        }

        //-------------------- Get cart by user id --------------------------------------------//

        public async Task<ServiceResponse<List<CartItemDto>>> GetCartByUserAsync(int userId)
        {
            var response = new ServiceResponse<List<CartItemDto>>();
            var cartItem = await (from c in _context.Carts
                                  join m in _context.Medicines
                                 on c.MedicineId equals m.Id
                                  where c.UserId == userId
                                  select new CartItemDto
                                  {
                                      CartId = c.Id,
                                      MedicineId = m.Id,
                                      MedicineName = m.Name,
                                      ImageUrl = m.ImageUrl,
                                      UnitPrice = c.UnitPrice,
                                      Discount = c.Discount,
                                      Quantity = c.Quantity,
                                      TotalPrice = c.TotalPrice
                                  }
                                 ).ToListAsync();
            response.Data = cartItem;
            response.Message = "Cart retrived successfully";
            return response;
        }


        //-------------------- Update cart by user id --------------------------------------------//

        public async Task<ServiceResponse<Cart>> UpdateCartAsync(int cartId, UpdateCartDto model)
        {
            var response = new ServiceResponse<Cart>();

            var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.Id == cartId);
            if (cartItem == null)
            {
                response.Success = false;
                response.Message = "Cart item not found.";
                return response;
            }

            cartItem.Quantity = model.Quantity;
            decimal price = cartItem.UnitPrice ?? 0;
            decimal discount = cartItem.Discount ?? 0;
            cartItem.TotalPrice = (price - discount) * model.Quantity;
            await _context.SaveChangesAsync();

            response.Data = cartItem;
            response.Message = "Cart updated successfully.";

            return response;
        }

        //-------------------- Remove Cart --------------------------------------------//
        public async Task<ServiceResponse<bool>> RemoveCartItemAsync(int cartId)
        {
            var response = new ServiceResponse<bool>();

            var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.Id == cartId);
            if (cartItem == null)
            {
                response.Success = false;
                response.Message = "Cart item not found.";
                return response;
            }
            _context.Carts.Remove(cartItem);

            await _context.SaveChangesAsync();

            response.Data = true;
            response.Message = "Item removed from cart.";

            return response;
        }
    }
}
