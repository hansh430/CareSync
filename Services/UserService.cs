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
                Type = "User",
                CreatedOn = DateTime.UtcNow,
                Status = 0,
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
        public async Task<ServiceResponse<UserDto>> LoginAsync(LoginDto model)
        {
            var response = new ServiceResponse<UserDto>();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
            if (user == null)
            {
                response.Success = false;
                response.Message = "Invalid email or password.";
                return response;
            }
            bool validPassword = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);

            if (!validPassword)
            {
                response.Success = false;
                response.Message = "Invalid email or password.";
                return response;
            }
            response.Data = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Fund = user.Fund,
                Type = user.Type,
                Status = user.Status
            };
            response.Message = "Login successful.";

            return response;
        }
    }


}
