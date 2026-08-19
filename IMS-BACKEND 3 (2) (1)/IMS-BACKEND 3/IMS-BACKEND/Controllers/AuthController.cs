using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IMSBackend.Contracts;
using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using IMSBackend.Services;
using IMSBackend.Interfaces;
using IMSBackend.Services.Authentication;
namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;



        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;



        private readonly ILoginHistoryService _loginHistoryService;





        private readonly ILogger<AuthController> _logger;
        private readonly PermissionService _permissionService;



        public AuthController(
        AppDbContext context,
        IConfiguration configuration,
        ILogger<AuthController> logger,
        PermissionService permissionService,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        ILoginHistoryService loginHistoryService,
        EmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _permissionService = permissionService;



            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _loginHistoryService = loginHistoryService;
            _emailService = emailService;
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var email = NormalizeEmail(dto.Email);

                var phone = new string(
                    dto.PhoneNumber
                        .Where(char.IsDigit)
                        .ToArray());

                // Check Users table
                var userExists = await _context.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Email == email, cancellationToken);

                // Check PendingUsers table
                var pendingUserExists = await _context.PendingUsers
                    .AsNoTracking()
                    .AnyAsync(u => u.Email == email, cancellationToken);

                if (userExists || pendingUserExists)
                {
                    return Conflict(ApiResponse<object>.Fail(
                        "An account with this email already exists.",
                        traceId: HttpContext.TraceIdentifier));
                }

                // Check phone in Users table
                var phoneExists = await _context.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.PhoneNumber == phone, cancellationToken);

                // Check phone in PendingUsers table
                var pendingPhoneExists = await _context.PendingUsers
                    .AsNoTracking()
                    .AnyAsync(u => u.PhoneNumber == phone, cancellationToken);

                if (phoneExists || pendingPhoneExists)
                {
                    return Conflict(ApiResponse<object>.Fail(
                        "An account with this phone number already exists.",
                        traceId: HttpContext.TraceIdentifier));
                }

                var newUser = new User
                {
                    Name = dto.Name.Trim(),
                    Email = email,
                    PhoneNumber = phone,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role,
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync(cancellationToken);

                return CreatedAtAction(
                    nameof(Register),
                    new { id = newUser.Id },
                    ApiResponse<object>.Ok(new
                    {
                        newUser.Id,
                        newUser.Name,
                        newUser.Email,
                        newUser.PhoneNumber,
                        newUser.Role,
                        newUser.IsActive
                    },
                    "Registration successful.",
                    HttpContext.TraceIdentifier));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }









        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto, CancellationToken cancellationToken)
        {
            var loginValue = dto.EmailOrPhone.Trim();



            var normalizedEmail = loginValue.ToLowerInvariant();



            var normalizedPhone = new string(
            loginValue
            .Where(char.IsDigit)
            .ToArray());



            var email = dto.EmailOrPhone.Trim().ToLower();
            var phone = new string(dto.EmailOrPhone.Where(char.IsDigit).ToArray());



            var user = await _context.Users
            .FirstOrDefaultAsync(x =>
            x.Email == email ||
            x.PhoneNumber == phone,
            cancellationToken);



            if (user == null)
            {
                return Unauthorized(ApiResponse<object>.Fail(
                "Invalid email/phone number or password.",
                traceId: HttpContext.TraceIdentifier));
            }



            if (!user.IsEmailVerified)
            {
                return Unauthorized(ApiResponse<object>.Fail(
                "Please verify your email before logging in.",
                traceId: HttpContext.TraceIdentifier));
            }



            if (!user.IsActive)
            {
                return Unauthorized(ApiResponse<object>.Fail(
                "Your account is inactive.",
                traceId: HttpContext.TraceIdentifier));
            }



            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;



                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                    user.FailedLoginAttempts = 0;



                    _context.Users.Update(user);
                    await _context.SaveChangesAsync(cancellationToken);



                    return Unauthorized(ApiResponse<object>.Fail(
                    "Your account has been locked for 15 minutes due to multiple failed login attempts.",
                    traceId: HttpContext.TraceIdentifier));
                }



                _context.Users.Update(user);
                await _context.SaveChangesAsync(cancellationToken);



                return Unauthorized(ApiResponse<object>.Fail(
                $"Invalid email/phone number or password. Remaining attempts: {5 - user.FailedLoginAttempts}",
                traceId: HttpContext.TraceIdentifier));
            }



            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                return Unauthorized(ApiResponse<object>.Fail(
                $"Your account is locked until {user.LockoutEnd:yyyy-MM-dd HH:mm:ss} UTC.",
                traceId: HttpContext.TraceIdentifier));
            }





            if (user.FailedLoginAttempts > 0 || user.LockoutEnd != null)
            {
                user.FailedLoginAttempts = 0;
                user.LockoutEnd = null;



                _context.Users.Update(user);
                await _context.SaveChangesAsync(cancellationToken);
            }


            var systemSettings = await _context.SystemSettings
    .AsNoTracking()
    .FirstOrDefaultAsync(cancellationToken);


            var permissions = await _permissionService.GetPermissionsAsync(user.Role);


            if (systemSettings != null && systemSettings.EnableTwoFactorAuth)
            {

                // Delete old unused login OTPs
                var oldOtps = await _context.Otps
                    .Where(x =>
                        x.UserId == user.Id &&
                        x.Purpose == "Login" &&
                        !x.IsUsed)
                    .ToListAsync(cancellationToken);

                _context.Otps.RemoveRange(oldOtps);

                await _context.SaveChangesAsync(cancellationToken);

                // Generate a 6-digit OTP
                var otpCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

                // Create OTP object
                var otp = new Otp
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Code = otpCode,
                    CreatedAt = DateTime.UtcNow,
                    ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                    IsUsed = false,
                    Purpose = "Login"
                };

                // Save OTP to database
                _context.Otps.Add(otp);
                await _context.SaveChangesAsync(cancellationToken);

                // Send OTP email
                var emailBody = $@"
        <h2>Login Verification</h2>

        <p>Hello {user.Name},</p>

        <p>Your login verification code is:</p>

        <h1>{otpCode}</h1>

        <p>This code is valid for 5 minutes.</p>

        <p>If you did not try to login, please ignore this email.</p>";

                await _emailService.SendEmailAsync(
                    user.Email,
                    "IMS Login Verification Code",
                    emailBody);

                return Ok(ApiResponse<object>.Ok(
                    new
                    {
                        requiresOtp = true,
                        userId = user.Id,
                        email = user.Email
                    },
                    "OTP sent successfully.",
                    HttpContext.TraceIdentifier));
            }


            var userAgent = Request.Headers["User-Agent"].ToString();



            await _loginHistoryService.RecordLoginAsync(
            user.Id,
            userAgent,
            HttpContext.Connection.RemoteIpAddress?.ToString());



            var accessToken = _jwtService.GenerateAccessToken(user);



            var refreshToken = _jwtService.GenerateRefreshToken(
            user.Id,
            HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            Request.Headers["User-Agent"].ToString()
            );



            await _refreshTokenService.SaveRefreshTokenAsync(refreshToken);



            var claims = new[]
            {
  new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
  new Claim(JwtRegisteredClaimNames.Email, user.Email),
  new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
  new Claim(ClaimTypes.Name, user.Email),
  new Claim(ClaimTypes.Role, user.Role),
  new Claim("UserId", user.Id.ToString())
  };







            var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing."))
            );



            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);



            var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
            );



            return Ok(ApiResponse<object>.Ok(
            new
            {
                token = accessToken,



                refreshToken = refreshToken.Token,



                expiresAt = DateTime.UtcNow.AddMinutes(15),



                user = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Role
                },



                permissions = permissions.Select(p => new
                {
                    moduleId = p.ModuleId,
                    moduleKey = p.Module.ModuleKey,
                    moduleName = p.Module.ModuleName,
                    p.CanView,
                    p.CanAdd,
                    p.CanEdit,
                    p.CanDelete
                })
            },
            "Login successful.",
            HttpContext.TraceIdentifier
            ));



        }







        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(
        RefreshTokenDto dto,
        CancellationToken cancellationToken)
        {
            // Find refresh token
            var storedToken = await _refreshTokenService
  .GetRefreshTokenAsync(dto.RefreshToken);



            if (storedToken == null)
            {
                return Unauthorized(ApiResponse<object>.Fail(
                "Invalid refresh token.",
                traceId: HttpContext.TraceIdentifier));
            }



            // Check revoked
            if (storedToken.RevokedAt != null)
            {
                return Unauthorized(ApiResponse<object>.Fail(
                "Refresh token has been revoked.",
                traceId: HttpContext.TraceIdentifier));
            }



            // Check expiry
            if (storedToken.ExpiresAt <= DateTime.UtcNow)
            {
                return Unauthorized(ApiResponse<object>.Fail(
                "Refresh token has expired.",
                traceId: HttpContext.TraceIdentifier));
            }



            // Load user
            var user = await _context.Users
  .FirstOrDefaultAsync(
  x => x.Id == storedToken.UserId,
  cancellationToken);



            if (user == null || !user.IsActive)
            {
                return Unauthorized(ApiResponse<object>.Fail(
                "User not found.",
                traceId: HttpContext.TraceIdentifier));
            }



            // Generate new JWT
            var accessToken = _jwtService.GenerateAccessToken(user);



            // Generate new Refresh Token
            var newRefreshToken = _jwtService.GenerateRefreshToken(
  user.Id,
  HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
  Request.Headers["User-Agent"].ToString());



            // Revoke old token
            storedToken.RevokedAt = DateTime.UtcNow;



            await _refreshTokenService.UpdateRefreshTokenAsync(storedToken);



            // Save new token
            await _refreshTokenService.SaveRefreshTokenAsync(newRefreshToken);



            return Ok(ApiResponse<object>.Ok(
            new RefreshTokenResponseDto
            {
                Token = accessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            },
            "Token refreshed successfully.",
            HttpContext.TraceIdentifier));
        }









        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto, CancellationToken cancellationToken)
        {
            var email = NormalizeEmail(dto.Email);
            var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(user => user.Email == email && user.IsActive, cancellationToken);



            if (userExists)
            {
                var otp = new Otp
                {
                    Email = email,
                    Code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString(),
                    ExpiryTime = DateTime.UtcNow.AddMinutes(5)
                };



                _context.Otps.Add(otp);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                "Password reset OTP generated for {Email}. TraceId: {TraceId}",
                email,
                HttpContext.TraceIdentifier);
            }



            return Ok(ApiResponse<object>.Ok(
            null,
            "If the email exists, a verification code has been generated.",
            HttpContext.TraceIdentifier));
        }









        [Authorize]
        [HttpPut("change-password/{userId}")]
        public async Task<IActionResult> ChangePassword(
        int userId,
        ChangePasswordDto dto,
        CancellationToken cancellationToken)
        {
            var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);



            if (user == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                "User not found.",
                traceId: HttpContext.TraceIdentifier));
            }



            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            {
                return BadRequest(ApiResponse<object>.Fail(
                "Current password is incorrect.",
                traceId: HttpContext.TraceIdentifier));
            }



            // Check confirm password
            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return BadRequest(ApiResponse<object>.Fail(
                "New password and confirm password do not match.",
                traceId: HttpContext.TraceIdentifier));
            }



            // Prevent using the same password again
            if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.PasswordHash))
            {
                return BadRequest(ApiResponse<object>.Fail(
                "New password cannot be the same as the current password.",
                traceId: HttpContext.TraceIdentifier));
            }





            if (dto.NewPassword.Length < 8)
            {
                return BadRequest(ApiResponse<object>.Fail(
                "Password must be at least 8 characters.",
                traceId: HttpContext.TraceIdentifier));
            }





            if (!dto.NewPassword.Any(char.IsUpper))
            {
                return BadRequest(ApiResponse<object>.Fail(
                "Password must contain at least one uppercase letter.",
                traceId: HttpContext.TraceIdentifier));
            }





            if (!dto.NewPassword.Any(char.IsLower))
            {
                return BadRequest(ApiResponse<object>.Fail(
                "Password must contain at least one lowercase letter.",
                traceId: HttpContext.TraceIdentifier));
            }





            if (!dto.NewPassword.Any(char.IsDigit))
            {
                return BadRequest(ApiResponse<object>.Fail(
                "Password must contain at least one number.",
                traceId: HttpContext.TraceIdentifier));
            }





            if (!dto.NewPassword.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return BadRequest(ApiResponse<object>.Fail(
                "Password must contain at least one special character.",
                traceId: HttpContext.TraceIdentifier));
            }





            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            // Invalidate all existing JWTs
            user.TokenVersion++;



            // Revoke all refresh tokens
            await _refreshTokenService.RevokeAllUserTokensAsync(userId);



            _context.AuditLogs.Add(new AuditLog
            {
                UserId = user.Id,
                Action = "Change Password",
                Module = "Authentication",
                TableName = "Users",
                RecordId = user.Id,
                Description = $"Password changed for user {user.Name}",
                CreatedAt = DateTime.UtcNow
            });





            await _loginHistoryService.RecordLogoutAllAsync(
            userId,
            "Password Changed");



            await _context.SaveChangesAsync(cancellationToken);



            return Ok(ApiResponse<object>.Ok(
            null,
            "Password changed successfully.",
            HttpContext.TraceIdentifier));
        }









       




        [Authorize]
        [HttpPost("logout/{userId}")]
        public async Task<IActionResult> Logout(
        int userId,
        CancellationToken cancellationToken)
        {
            await _loginHistoryService.RecordLogoutAsync(
            userId,
            "Manual");



            return Ok(ApiResponse<object>.Ok(
            null,
            "Logged out successfully.",
            HttpContext.TraceIdentifier));
        }





        [Authorize]
        [HttpPost("logout-all-devices/{userId}")]
        public async Task<IActionResult> LogoutAllDevices(
        int userId,
        CancellationToken cancellationToken)
        {
            var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);



            if (user == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                "User not found.",
                traceId: HttpContext.TraceIdentifier));
            }



            // Invalidate every existing JWT
            user.TokenVersion++;



            // Revoke all refresh tokens
            await _refreshTokenService.RevokeAllUserTokensAsync(userId);



            // Optional: deactivate UserTokens if you're still using that table
            var tokens = await _context.UserTokens
  .Where(x => x.UserId == userId && x.IsActive)
  .ToListAsync(cancellationToken);



            foreach (var token in tokens)
            {
                token.IsActive = false;
            }



            await _loginHistoryService.RecordLogoutAllAsync(
            userId,
            "Logout All Devices");



            await _context.SaveChangesAsync(cancellationToken);



            return Ok(ApiResponse<object>.Ok(
            null,
            "Logged out from all devices successfully.",
            HttpContext.TraceIdentifier));
        }







        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto, CancellationToken cancellationToken)
        {
            var email = NormalizeEmail(dto.Email);
            var otp = await _context.Otps
            .Where(item => item.Email == email && item.Code == dto.Otp)
            .OrderByDescending(item => item.ExpiryTime)
            .FirstOrDefaultAsync(cancellationToken);



            if (otp == null || otp.ExpiryTime < DateTime.UtcNow)
            {
                return BadRequest(ApiResponse<object>.Fail(
                "Invalid or expired verification code.",
                traceId: HttpContext.TraceIdentifier));
            }



            var user = await _context.Users.FirstOrDefaultAsync(item => item.Email == email, cancellationToken);



            if (user == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                "Invalid or expired verification code.",
                traceId: HttpContext.TraceIdentifier));
            }



            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            _context.Otps.RemoveRange(_context.Otps.Where(item => item.Email == email));



            user.TokenVersion++;



            await _refreshTokenService.RevokeAllUserTokensAsync(user.Id);



            await _loginHistoryService.RecordLogoutAllAsync(
            user.Id,
            "Password Reset");



            await _context.SaveChangesAsync(cancellationToken);



            return Ok(ApiResponse<object>.Ok(
            null,
            "Password reset successful.",
            HttpContext.TraceIdentifier));
        }





        [Authorize]
        [HttpGet("claims")]
        public IActionResult GetClaims()
        {
            return Ok(User.Claims.Select(x => new
            {
                x.Type,
                x.Value
            }));
        }



        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(
    VerifyOtpDto dto,
    CancellationToken cancellationToken)
        {
            // Find OTP
            var otp = await _context.Otps
                .Where(x =>
                    x.UserId == dto.UserId &&
                    x.Code == dto.Otp &&
                    x.Purpose == "Login" &&
                    !x.IsUsed)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (otp == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Invalid OTP.",
                    traceId: HttpContext.TraceIdentifier));
            }

            // Check expiry
            if (otp.ExpiryTime < DateTime.UtcNow)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "OTP has expired.",
                    traceId: HttpContext.TraceIdentifier));
            }

            // Mark OTP as used
            otp.IsUsed = true;

            // Get user
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == dto.UserId, cancellationToken);

            if (user == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "User not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            // Permissions
            var permissions = await _permissionService.GetPermissionsAsync(user.Role);

            // Record login
            await _loginHistoryService.RecordLoginAsync(
                user.Id,
                Request.Headers["User-Agent"].ToString(),
                HttpContext.Connection.RemoteIpAddress?.ToString());

            // Generate JWT
            var accessToken = _jwtService.GenerateAccessToken(user);

            // Generate Refresh Token
            var refreshToken = _jwtService.GenerateRefreshToken(
                user.Id,
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                Request.Headers["User-Agent"].ToString());

            await _refreshTokenService.SaveRefreshTokenAsync(refreshToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(ApiResponse<object>.Ok(
                new
                {
                    token = accessToken,
                    refreshToken = refreshToken.Token,
                    expiresAt = DateTime.UtcNow.AddHours(8),

                    user = new
                    {
                        user.Id,
                        user.Name,
                        user.Email,
                        user.Role
                    },

                    permissions = permissions.Select(p => new
                    {
                        moduleId = p.ModuleId,
                        moduleKey = p.Module.ModuleKey,
                        moduleName = p.Module.ModuleName,
                        p.CanView,
                        p.CanAdd,
                        p.CanEdit,
                        p.CanDelete
                    })
                },
                "Login successful.",
                HttpContext.TraceIdentifier));
        }


        private static string NormalizeEmail(string email)
        => email.Trim().ToLowerInvariant();



        [HttpPost("verify-email-otp")]
        public async Task<IActionResult> VerifyEmailOtp(
    VerifyEmailOtpDto dto,
    CancellationToken cancellationToken)
        {
            var email = NormalizeEmail(dto.Email);

            // Find the latest unused email verification OTP
            var otp = await _context.Otps
                .Where(x =>
                    x.Email == email &&
                    x.Code == dto.Otp &&
                    x.Purpose == "EmailVerification" &&
                    !x.IsUsed)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (otp == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Invalid OTP.",
                    traceId: HttpContext.TraceIdentifier));
            }

            // Check expiry
            if (otp.ExpiryTime < DateTime.UtcNow)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "OTP has expired.",
                    traceId: HttpContext.TraceIdentifier));
            }

            // Find pending user
            var pendingUser = await _context.PendingUsers
                .FirstOrDefaultAsync(
                    x => x.Email == email,
                    cancellationToken);

            if (pendingUser == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "No pending account found for this email.",
                    traceId: HttpContext.TraceIdentifier));
            }

            // Mark OTP as used
            otp.IsUsed = true;

            // Create actual user
            var user = new User
            {
                Name = pendingUser.Name,
                Email = pendingUser.Email,
                PhoneNumber = pendingUser.PhoneNumber,
                PasswordHash = pendingUser.PasswordHash,
                Role = pendingUser.Role,
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TokenVersion = 1
            };

            _context.Users.Add(user);

            // Remove pending user
            _context.PendingUsers.Remove(pendingUser);

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(ApiResponse<object>.Ok(
                null,
                "Email verified successfully. You can now login.",
                HttpContext.TraceIdentifier));
        }


        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerificationEmail(
    ResendVerificationDto dto,
    CancellationToken cancellationToken)
        {
            var email = NormalizeEmail(dto.Email);

            var pendingUser = await _context.PendingUsers
                .FirstOrDefaultAsync(
                    x => x.Email == email,
                    cancellationToken);

            if (pendingUser == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "No pending account matches that email.",
                    traceId: HttpContext.TraceIdentifier));
            }

            // Delete old unused email verification OTPs
            var oldOtps = await _context.Otps
                .Where(x =>
                    x.Email == email &&
                    x.Purpose == "EmailVerification" &&
                    !x.IsUsed)
                .ToListAsync(cancellationToken);

            _context.Otps.RemoveRange(oldOtps);

            // Generate new OTP
            var otpCode = RandomNumberGenerator
                .GetInt32(100000, 1000000)
                .ToString();

            var otp = new Otp
            {
                Email = email,
                Code = otpCode,
                CreatedAt = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                Purpose = "EmailVerification"
            };

            _context.Otps.Add(otp);

            await _context.SaveChangesAsync(cancellationToken);

            var emailBody = $@"
        <h2>Email Verification</h2>

        <p>Hello {pendingUser.Name},</p>

        <p>Your email verification OTP is:</p>

        <h1>{otpCode}</h1>

        <p>This OTP is valid for 10 minutes.</p>

        <p>If you did not register for an account, please ignore this email.</p>";

            await _emailService.SendEmailAsync(
                email,
                "Email Verification OTP",
                emailBody);

            return Ok(ApiResponse<object>.Ok(
                null,
                "Verification OTP sent successfully.",
                HttpContext.TraceIdentifier));
        }



    }
}







