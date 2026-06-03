using CareSync.Data;
using CareSync.Dtos;
using CareSync.Models;
using CareSync.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly EmedicineContext _context;
        private readonly IUserService _userService;
        private readonly IAdminService _adminservice;

        public UsersController(IConfiguration configuration, EmedicineContext context, IUserService userService, IAdminService adminService)
        {
            _configuration = configuration;
            _context = context;
            _userService = userService;
            _adminservice = adminService;
        }

        //-------------------- User Registration --------------------------------------------//

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var result = await _userService.RegisterAsync(model);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        //-------------------- User Login --------------------------------------------//

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var result = await _userService.LoginAsync(model);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("admin-login")]
        public async Task<IActionResult> AdminLogin(AdminLoginDto model)
        {
            var result =
                await _adminservice.AdminLoginAsync(model);

            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(result);
        }

        //-------------------- Get All Users --------------------------------------------//
        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _userService.GetUsersAsync();

            return Ok(result);
        }

        //-------------------- Get User by ID --------------------------------------------//

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result);
        }

        //-------------------- Update User Details --------------------------------------------//

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto model)
        {
            var result = await _userService.UpdateUserAsync(id, model);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}
