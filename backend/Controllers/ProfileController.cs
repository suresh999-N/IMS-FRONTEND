

using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using IMS.Backend.Helpers;

namespace IMSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        // GET PROFILE
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfile(int userId)
        {
            var user = await _context.Users
                .Where(x => x.Id == userId)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Email,
                    x.PhoneNumber,
                    x.EmployeeId,
                    x.Department,
                    x.Role,
                    x.Warehouse,
                    x.ProfilePhoto,
                    x.IsActive,
                    x.LastLogin
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }




        // me - GET MY PROFILE

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var user = await _context.Users
                .Where(x => x.Id == userId)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Email,
                    x.PhoneNumber,
                    x.EmployeeId,
                    x.Department,
                    x.Role,
                    x.Warehouse,
                    x.ProfilePhoto,
                    x.IsActive,
                    x.LastLogin
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        // UPDATE PROFILE
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateProfile(
            int userId,
            UpdateProfileDto dto,
            CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(
                new object[] { userId },
                cancellationToken);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Name Validation
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Name is required");
            }

            // Manual Name Regex Validation
            if (!EmailValidationHelper.IsValidName(dto.Name))
            {
                return BadRequest("Name can contain only letters and spaces.");
            }

            var email = dto.Email.Trim().ToLowerInvariant();
            if (!EmailValidationHelper.IsValidEmail(email))
            {
                return BadRequest("Enter a valid email address.");
            }
            var phone = dto.PhoneNumber?.Trim();

            // Duplicate Email Check
            var emailExists = await _context.Users.AnyAsync(
                x => x.Email == email && x.Id != userId,
                cancellationToken);

            if (emailExists)
            {
                return BadRequest("Email already exists");
            }

            // Update Fields
            user.Name = dto.Name.Trim();
            user.Email = email;
            user.PhoneNumber = phone;
            user.EmployeeId = dto.EmployeeId;
            user.Department = dto.Department;
            user.Warehouse = dto.Warehouse;
            user.ProfilePhoto = dto.ProfilePhoto;
            user.UpdatedAt = DateTime.Now;

            // Audit Log
            _context.AuditLogs.Add(new AuditLog
            {
                UserId = user.Id,
                Action = "Update",
                Module = "Profile",
                TableName = "Users",
                RecordId = user.Id,
                Description = $"Profile updated by {user.Name}",
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new
            {
                message = "Profile updated successfully"
            });
        }


        // UPLOAD PROFILE PHOTO
        [HttpPost("upload-photo/{userId}")]
        public async Task<IActionResult> UploadPhoto(int userId, IFormFile file)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }


            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("Only JPG, JPEG and PNG files are allowed");
            }

            if (file.Length > 2 * 1024 * 1024)
            {
                return BadRequest("Maximum file size is 2MB");
            }


            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/profilephotos"
            );

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Guid.NewGuid().ToString()
                + Path.GetExtension(file.FileName);

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }


            if (!string.IsNullOrEmpty(user.ProfilePhoto))
            {
                var oldFilePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    user.ProfilePhoto.TrimStart('/')
                        .Replace("/", Path.DirectorySeparatorChar.ToString())
                );

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }



            user.ProfilePhoto = "/profilephotos/" + fileName;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Photo uploaded successfully",
                photoUrl = user.ProfilePhoto
            });
        }




        [HttpDelete("photo/{userId}")]
        public async Task<IActionResult> DeletePhoto(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (string.IsNullOrEmpty(user.ProfilePhoto))
            {
                return BadRequest("No profile photo found");
            }


            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                 "wwwroot",
                 user.ProfilePhoto.TrimStart('/')
                    .Replace("/", Path.DirectorySeparatorChar.ToString())
);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            user.ProfilePhoto = null;
            user.UpdatedAt = DateTime.Now;

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = user.Id,
                Action = "Delete Photo",
                Module = "Profile",
                TableName = "Users",
                RecordId = user.Id,
                Description = $"Profile photo deleted by {user.Name}",
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile photo deleted successfully"
            });
        }



        // LOGOUT ALL DEVICES
        [HttpPost("logout-all-devices/{userId}")]
        public async Task<IActionResult> LogoutAllDevices(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = "LOGOUT_ALL_DEVICES",
                Module = "Authentication",
                TableName = "Users",
                RecordId = userId,
                Description = $"{user.Name} logged out from all devices",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Logged out from all devices successfully"
            });
        }




    }
}