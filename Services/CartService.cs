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
        public async Task<ServiceResponse<Cart>> AddToCartAsync(AddCartDto model)
        {
            var response = new ServiceResponse<Cart>();
            var medicine = await _context.Medicines.FirstOrDefaultAsync(m => m.Id == model.MedicineId);

            if (medicine == null)
            {
                response.Success = false;
                response.Message = "Medicine not found.";
                return response;
            }

            // Check if medicine already exists in cart
            var existingCartItem = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == model.UserId && c.MedicineId == model.MedicineId);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += model.Quantity;

                existingCartItem.TotalPrice =
                    existingCartItem.Quantity *
                    existingCartItem.UnitPrice;

                await _context.SaveChangesAsync();

                response.Data = existingCartItem;
                response.Message = "Cart updated.";
                return response;
            }

            var cart = new Cart
            {
                UserId = model.UserId,
                MedicineId = model.MedicineId,
                Quantity = model.Quantity,
                UnitPrice = medicine.UnitPrice,
                Discount = 0,
                TotalPrice = medicine.UnitPrice * model.Quantity
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            response.Data = cart;
            response.Message = "Added to cart.";

            return response;
        }
    }
}
