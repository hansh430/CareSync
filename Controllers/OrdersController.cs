using CareSync.Helpers;
using CareSync.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        //--------------------place order -------------------------------------

        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder()
        {
            int userId = UserHelper.GetUserId(User);

            var result = await _orderService.PlaceOrderAsync(userId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        //--------------------Get order details for a user -------------------------------------
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            int userId = UserHelper.GetUserId(User);
            var result = await _orderService.GetOrderByUserAsync(userId);
            return Ok(result);
        }

        //--------------------Get order details by order id -------------------------------------
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var result = await _orderService.GetOrderDetailsAsync(orderId);
            return Ok(result);
        }
    }
}
