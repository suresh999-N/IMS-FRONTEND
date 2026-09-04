using IMSBackend.Data;
using IMSBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IMSBackend.DTOs;
using IMSBackend.Services;
using System.Security.Claims;
using IMS.Backend.Helpers;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PermissionService _permissionService;

        private const string UsersModuleKey = "users";

        public UsersController(
            AppDbContext context,
            PermissionService permissionService)
        {
            _context = context;
            _permissionService = permissionService;
        }

        // ============================================================
        // GET ALL USERS
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var currentUserRole = GetCurrentUserRole();

            if (string.IsNullOrWhiteSpace(currentUserRole))
            {
                return Unauthorized(new
                {
                    message = "User role not found in token."
                });
            }

            var hasViewPermission = await _permissionService.HasPermission(
                currentUserRole,
                UsersModuleKey,
                "view");

            if (!hasViewPermission)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "You do not have permission to view users."
                });
            }

            var roleStatusDict = await _context.Roles
                .AsNoTracking()
                .ToDictionaryAsync(r => r.RoleName, r => r.IsActive, StringComparer.OrdinalIgnoreCase);

            var rawUsers = await _context.Users
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

            var users = rawUsers.Select(x =>
            {
                var roleIsActive = roleStatusDict.TryGetValue(x.Role, out var rActive) ? rActive : true;
                var effectiveIsActive = x.IsActive && roleIsActive;
                return new
                {
                    x.Id,
                    x.Name,
                    x.Email,
                    x.PhoneNumber,
                    x.Role,
                    IsActive = x.IsActive,
                    UserIsActive = x.IsActive,
                    RoleIsActive = roleIsActive,
                    EffectiveIsActive = effectiveIsActive
                };
            }).ToList();

            return Ok(users);
        }

        // ============================================================
        // GET USER BY ID
        // ============================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var currentUserRole = GetCurrentUserRole();

            if (string.IsNullOrWhiteSpace(currentUserRole))
            {
                return Unauthorized(new
                {
                    message = "User role not found in token."
                });
            }

            var hasViewPermission = await _permissionService.HasPermission(
                currentUserRole,
                UsersModuleKey,
                "view");

            if (!hasViewPermission)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "You do not have permission to view users."
                });
            }

            var rawUser = await _context.Users
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

            if (rawUser == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            var roleIsActive = await _context.Roles
                .AsNoTracking()
                .Where(r => r.RoleName.ToLower() == rawUser.Role.ToLower())
                .Select(r => r.IsActive)
                .FirstOrDefaultAsync();

            var effectiveIsActive = rawUser.IsActive && roleIsActive;

            var user = new
            {
                rawUser.Id,
                rawUser.Name,
                rawUser.Email,
                rawUser.PhoneNumber,
                rawUser.Role,
                IsActive = effectiveIsActive,
                UserIsActive = rawUser.IsActive,
                RoleIsActive = roleIsActive,
                EffectiveIsActive = effectiveIsActive
            };

            return Ok(user);
        }

        // ============================================================
        // CREATE USER
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Name) && !EmailValidationHelper.IsValidName(dto.Name))
            {
                return BadRequest(new { message = "Full name must contain only letters and spaces." });
            }
            var email = dto.Email.Trim().ToLowerInvariant();
            if (!EmailValidationHelper.IsValidEmail(email))
            {
                return BadRequest(new { message = "Enter a valid email address." });
            }
            var currentUserRole = GetCurrentUserRole();

            if (string.IsNullOrWhiteSpace(currentUserRole))
            {
                return Unauthorized(new
                {
                    message = "User role not found in token."
                });
            }

            var hasAddPermission = await _permissionService.HasPermission(
                currentUserRole,
                UsersModuleKey,
                "add");

            if (!hasAddPermission)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "You do not have permission to create users."
                });
            }

            var phone = dto.PhoneNumber.Trim();

            var emailExists = await _context.Users
                .AnyAsync(x => x.Email == email);

            if (emailExists)
            {
                return BadRequest(new
                {
                    message = "Email already exists."
                });
            }

            var phoneExists = await _context.Users
                .AnyAsync(x => x.PhoneNumber == phone);

            if (phoneExists)
            {
                return BadRequest(new
                {
                    message = "Phone number already exists."
                });
            }

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
                Email = email,
                PhoneNumber = phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                IsActive = dto.IsActive,
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

        // ============================================================
        // UPDATE USER
        // ============================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(
            int id,
            UpdateUserDto dto)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            if (currentUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Current user could not be identified."
                });
            }

            if (string.IsNullOrWhiteSpace(currentUserRole))
            {
                return Unauthorized(new
                {
                    message = "User role not found in token."
                });
            }

            // Only users with CanEdit permission can edit users.
            var hasEditPermission = await _permissionService.HasPermission(
                currentUserRole,
                UsersModuleKey,
                "edit");

            if (!hasEditPermission)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "You do not have permission to edit users."
                });
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            // ========================================================
            // PREVENT CURRENT LOGGED-IN USER FROM CHANGING
            // THEIR OWN ACTIVE / INACTIVE STATUS
            //
            // Admin can still edit their own name, email, phone,
            // and role. Only their own status change is blocked.
            // ========================================================
            if (currentUserId.Value == id && !dto.IsActive)
            {
                return BadRequest(new
                {
                    message = "You cannot deactivate your own account."
                });
            }

            if (!string.IsNullOrWhiteSpace(dto.Name) && !EmailValidationHelper.IsValidName(dto.Name))
            {
                return BadRequest(new { message = "Full name must contain only letters and spaces." });
            }

            var email = dto.Email.Trim().ToLowerInvariant();
            if (!EmailValidationHelper.IsValidEmail(email))
            {
                return BadRequest(new { message = "Enter a valid email address." });
            }
            var phone = dto.PhoneNumber.Trim();

            var emailExists = await _context.Users
                .AnyAsync(x =>
                    x.Email == email &&
                    x.Id != id);

            if (emailExists)
            {
                return BadRequest(new
                {
                    message = "Email already exists."
                });
            }

            var phoneExists = await _context.Users
                .AnyAsync(x =>
                    x.PhoneNumber == phone &&
                    x.Id != id);

            if (phoneExists)
            {
                return BadRequest(new
                {
                    message = "Phone number already exists."
                });
            }

            user.Name = dto.Name.Trim();
            user.Email = email;
            user.PhoneNumber = phone;
            user.Role = dto.Role;
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User updated successfully."
            });
        }

        // ============================================================
        // DELETE USER
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            if (currentUserId == null)
            {
                return Unauthorized(new
                {
                    message = "Current user could not be identified."
                });
            }

            if (string.IsNullOrWhiteSpace(currentUserRole))
            {
                return Unauthorized(new
                {
                    message = "User role not found in token."
                });
            }

            var hasDeletePermission = await _permissionService.HasPermission(
                currentUserRole,
                UsersModuleKey,
                "delete");

            if (!hasDeletePermission)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "You do not have permission to delete users."
                });
            }

            // Current logged-in user can NEVER delete himself.
            if (currentUserId.Value == id)
            {
                return BadRequest(new
                {
                    message = "You cannot delete your own account."
                });
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

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
                message = "User deleted successfully."
            });
        }

        // ============================================================
        // GET CURRENT USER ID FROM JWT
        // ============================================================
        private int? GetCurrentUserId()
        {
            var userIdClaim =
                User.FindFirst("UserId")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return null;
        }

        // ============================================================
        // GET CURRENT USER ROLE FROM JWT
        // ============================================================
        private string? GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}