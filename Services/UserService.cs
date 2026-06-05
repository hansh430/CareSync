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
        private readonly ITokenService _tokenService;
        public UserService(EmedicineContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        //-------------------- Registration --------------------------------------------//
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

        //-------------------- Login --------------------------------------------//

        public async Task<ServiceResponse<LoginResponseDto>> LoginAsync(LoginDto model)
        {
            var response = new ServiceResponse<LoginResponseDto>();

            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Email == model.Email);

            if (user == null)
            {
                response.Success = false;
                response.Message = "Invalid email or password.";

                return response;
            }

            bool validPassword =
                BCrypt.Net.BCrypt.Verify(
                    model.Password,
                    user.Password);

            if (!validPassword)
            {
                response.Success = false;
                response.Message = "Invalid email or password.";

                return response;
            }

            var token = _tokenService.CreateToken(user);

            response.Data = new LoginResponseDto
            {
                Token = token,

                User = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Fund = user.Fund,
                    Type = user.Type,
                    Status = user.Status
                }
            };

            response.Message = "Login successful.";

            return response;
        }

        //-------------------- Get User by ID --------------------------------------------//
        public async Task<ServiceResponse<UserDto>> GetUserByIdAsync(int id)
        {
            var response = new ServiceResponse<UserDto>();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }
            response.Data = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            return response;
        }

        //-------------------- update User --------------------------------------------//
        public async Task<ServiceResponse<UserDto>> UpdateUserAsync(int id, UpdateUserDto model)
        {
            var response = new ServiceResponse<UserDto>();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }
            // Optional: Prevent duplicate emails
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                bool emailExists = await _context.Users
                    .AnyAsync(u => u.Email == model.Email && u.Id != id);

                if (emailExists)
                {
                    response.Success = false;
                    response.Message = "Email already exists.";
                    return response;
                }
            }
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            await _context.SaveChangesAsync();

            response.Data = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            response.Message = "Profile updated successfully.";
            return response;
        }


    }

}
