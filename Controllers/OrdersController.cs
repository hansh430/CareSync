using CareSync.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("place-order/{userId}")]
        public async Task<IActionResult> PlaceOrder(int userId)
        {
            var result = await _orderService.PlaceOrderAsync(userId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}
