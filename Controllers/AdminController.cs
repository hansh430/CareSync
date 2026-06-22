using CareSync.Dtos;
using CareSync.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var result = await _adminService.GetDashboardAsync();

            return Ok(result);
        }

        //-------------------- Get All Users --------------------------------------------//
        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _adminService.GetUsersAsync();

            return Ok(result);
        }

        //-------------------- Add Fund to Users --------------------------------------------//
        [HttpPut("users/{id}/fund")]
        public async Task<IActionResult> AddFund(int id, FundDto model)
        {
            var result = await _adminService.AddFundAsync(id, model);
            return Ok(result);
        }

        //-------------------- Fetch all orders --------------------------------------------//
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            return Ok(await _adminService.GetOrdersAsync());
        }

        //-------------------- Fetch all order Details --------------------------------------------//

        [HttpGet("orders/{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var result = await _adminService.GetOrderDetailsAsync(orderId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        //-------------------- Update order status --------------------------------------------//
        [HttpPut("orders/{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusDto model)
        {
            var result = await _adminService.UpdateOrderStatusAsync(id, model);

            return Ok(result);
        }
    }
}
