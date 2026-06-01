using CareSync.Dtos;
using CareSync.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var result = await _cartService.AddToCartAsync(model);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}
