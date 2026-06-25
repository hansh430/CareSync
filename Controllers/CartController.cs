using CareSync.Dtos;
using CareSync.Helpers;
using CareSync.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Security.Claims;

namespace CareSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("addToCart")]
        public async Task<IActionResult> AddToCart(AddCartDto model)
        {
            var userId = UserHelper.GetUserId(User);

            var result = await _cartService.AddToCartAsync(userId, model);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet()]
        public async Task<IActionResult> GetCart()
        {
            var userId = UserHelper.GetUserId(User);
            var result = await _cartService.GetCartByUserAsync(userId);

            return Ok(result);
        }

        [HttpPut("{cartId}")]
        public async Task<IActionResult> UpdateCart(int cartId, UpdateCartDto model)
        {
            var result = await _cartService.UpdateCartAsync(cartId, model);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpDelete("{cartId}")]
        public async Task<IActionResult> RemoveCartItem(int cartId)
        {
            var result = await _cartService.RemoveCartItemAsync(cartId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}
