using CareSync.Common;
using CareSync.Data;
using CareSync.Dtos;
using CareSync.Models;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Services
{
    public class UserService : IUserService
    {
        private readonly EmedicineContext _context;

        public UserService(EmedicineContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<UserDto>> RegisterAsync(RegisterDto model)
        {
            var response = new ServiceResponse<UserDto>();
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
            if (existingUser != null)
            {
                response.Success = false;
                response.Message = "Email already registered.";
                return response;
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Type="User",
                CreatedOn = DateTime.UtcNow,
                Status = 1,
                Fund = 0
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            response.Data = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            response.Message = "Registration successful.";
            return response;
        }
    }
}
