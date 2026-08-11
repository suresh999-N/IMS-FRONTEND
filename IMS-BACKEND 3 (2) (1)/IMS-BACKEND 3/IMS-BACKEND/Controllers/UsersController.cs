using IMSBackend.Data;
using IMSBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IMSBackend.DTOs;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL USERS
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Email,
                    x.PhoneNumber,
                    x.Role,
                    x.IsActive
                })
                .ToListAsync();

            return Ok(users);
        }

        // =========================
        // GET USER BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Email,
                    x.PhoneNumber,
                    x.Role,
                    x.IsActive
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found"
                });
            }

            return Ok(user);
        }

        // =========================
        // =========================
        // CREATE USER
        // =========================
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            // Check email already exists
            var emailExists = await _context.Users
                .AnyAsync(x => x.Email == dto.Email.Trim().ToLower());

            if (emailExists)
            {
                return BadRequest(new
                {
                    message = "Email already exists."
                });
            }

            // Check phone number already exists
            var phoneExists = await _context.Users
                .AnyAsync(x => x.PhoneNumber == dto.PhoneNumber);

            if (phoneExists)
            {
                return BadRequest(new
                {
                    message = "Phone number already exists."
                });
            }

            // Password confirmation
            if (dto.Password != dto.ConfirmPassword)
            {
                return BadRequest(new
                {
                    message = "Password and Confirm Password do not match."
                });
            }

            var user = new User
            {
                Name = dto.Name.Trim(),
                Email = dto.Email.Trim().ToLower(),
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,

                IsActive = dto.IsActive,

                // IMPORTANT: Admin-created users are already verified
                IsEmailVerified = true,

                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TokenVersion = 1
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User created successfully."
            });
        }

        // =========================
        // UPDATE USER
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(
    int id,
    UpdateUserDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            // Check duplicate email
            var emailExists = await _context.Users
                .AnyAsync(x =>
                    x.Email == dto.Email &&
                    x.Id != id);

            if (emailExists)
            {
                return BadRequest(new
                {
                    message = "Email already exists."
                });
            }

            // Check duplicate phone number
            var phoneExists = await _context.Users
                .AnyAsync(x =>
                    x.PhoneNumber == dto.PhoneNumber &&
                    x.Id != id);

            if (phoneExists)
            {
                return BadRequest(new
                {
                    message = "Phone number already exists."
                });
            }

            user.Name = dto.Name.Trim();
            user.Email = dto.Email.Trim().ToLower();
            user.PhoneNumber = dto.PhoneNumber;
            user.Role = dto.Role;
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User updated successfully."
            });
        }

        // =========================
        // DELETE USER
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found"
                });
            }

            // Delete login history first
            var loginHistory = await _context.LoginHistories
                .Where(x => x.UserId == id)
                .ToListAsync();

            if (loginHistory.Any())
            {
                _context.LoginHistories.RemoveRange(loginHistory);
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User deleted successfully"
            });
        }
    }
}
