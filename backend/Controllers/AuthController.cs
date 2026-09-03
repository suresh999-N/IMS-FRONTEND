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
        public async Task<IActionResult> Register(
            RegisterDto dto,
            CancellationToken cancellationToken)
        {
            try
            {
                var nameTrimmed = dto.Name?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(nameTrimmed))
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        "Full name is required.",
                        new Dictionary<string, string[]>
                        {
                            { "Name", new[] { "Full name is required." } }
                        },
                        traceId: HttpContext.TraceIdentifier));
                }

                if (nameTrimmed.Length < 2)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        "Full name must contain at least 2 characters.",
                        new Dictionary<string, string[]>
                        {
                            { "Name", new[] { "Full name must contain at least 2 characters." } }
                        },
                        traceId: HttpContext.TraceIdentifier));
                }

                if (nameTrimmed.Length > 50)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        "Full name cannot exceed 50 characters.",
                        new Dictionary<string, string[]>
                        {
                            { "Name", new[] { "Full name cannot exceed 50 characters." } }
                        },
                        traceId: HttpContext.TraceIdentifier));
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(nameTrimmed, @"^[a-zA-Z\p{L}]+(?:\s[a-zA-Z\p{L}]+)*$"))
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        "Full name must contain only letters and spaces.",
                        new Dictionary<string, string[]>
                        {
                            { "Name", new[] { "Full name must contain only letters and spaces." } }
                        },
                        traceId: HttpContext.TraceIdentifier));
                }

                var rawEmail = dto.Email?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(rawEmail))
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        "Email address is required.",
                        new Dictionary<string, string[]>
                        {
                            { "Email", new[] { "Email address is required." } }
                        },
                        traceId: HttpContext.TraceIdentifier));
                }

                var email = rawEmail.ToLowerInvariant();
                if (email.Contains("..") || !System.Text.RegularExpressions.Regex.IsMatch(email, @"^(?!\.)(?!.*\.\.)[a-zA-Z0-9._%+-]+(?<!\.)@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,24}$"))
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        "Enter a valid email address.",
                        new Dictionary<string, string[]>
                        {
                            { "Email", new[] { "Enter a valid email address." } }
                        },
                        traceId: HttpContext.TraceIdentifier));
                }

                var phone = dto.PhoneNumber?.Trim() ?? string.Empty;

                // Cleanup expired pending users
                var expiredPendingUsers = await _context.PendingUsers
                    .Where(u =>
                        (u.Email == email || u.PhoneNumber == phone) &&
                        u.EmailVerificationTokenExpiry < DateTime.UtcNow)
                    .ToListAsync(cancellationToken);

                if (expiredPendingUsers.Any())
                {
                    var expiredEmails = expiredPendingUsers
                        .Select(u => u.Email)
                        .ToList();

                    var associatedOtps = await _context.Otps
                        .Where(o => expiredEmails.Contains(o.Email))
                        .ToListAsync(cancellationToken);

                    _context.Otps.RemoveRange(associatedOtps);
                    _context.PendingUsers.RemoveRange(expiredPendingUsers);

                    await _context.SaveChangesAsync(cancellationToken);
                }

                // Uniqueness check for Email
                var userExists = await _context.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Email == email, cancellationToken);

                var pendingUserExists = await _context.PendingUsers
                    .AsNoTracking()
                    .AnyAsync(u => u.Email == email, cancellationToken);

                if (userExists || pendingUserExists)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        "Email address is already registered.",
                        new Dictionary<string, string[]>
                        {
                            { "Email", new[] { "Email address is already registered." } }
                        },
                        traceId: HttpContext.TraceIdentifier));
                }

                // Uniqueness check for Phone Number
                var phoneExists = await _context.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.PhoneNumber == phone, cancellationToken);

                var pendingPhoneExists = await _context.PendingUsers
                    .AsNoTracking()
                    .AnyAsync(u => u.PhoneNumber == phone, cancellationToken);

                if (phoneExists || pendingPhoneExists)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        "Mobile number is already registered.",
                        new Dictionary<string, string[]>
                        {
                            { "PhoneNumber", new[] { "Mobile number is already registered." } }
                        },
                        traceId: HttpContext.TraceIdentifier));
                }

                // Create Pending User
                var pendingUser = new PendingUser
                {
                    Name = dto.Name.Trim(),
                    Email = email,
                    PhoneNumber = phone,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role.Trim(),
                    EmailVerificationToken = Guid.NewGuid().ToString(),
                    EmailVerificationTokenExpiry = DateTime.UtcNow.AddMinutes(10)
                };

                _context.PendingUsers.Add(pendingUser);
                await _context.SaveChangesAsync(cancellationToken);

                // Generate 6-digit OTP
                var otpCode = RandomNumberGenerator
                    .GetInt32(100000, 1000000)
                    .ToString();

                var otp = new Otp
                {
                    Email = pendingUser.Email,
                    Code = otpCode,
                    CreatedAt = DateTime.UtcNow,
                    ExpiryTime = DateTime.UtcNow.AddMinutes(10),
                    IsUsed = false,
                    Purpose = "EmailVerification"
                };

                _context.Otps.Add(otp);
                await _context.SaveChangesAsync(cancellationToken);

                // Email Body
                var emailBody = $@"
<h2>Email Verification</h2>
<p>Hello {pendingUser.Name},</p>
<p>Your verification OTP is:</p>
<h1 style='color:blue'>{otpCode}</h1>
<p>This OTP is valid for 10 minutes.</p>
<p>Please do not share this OTP with anyone.</p>";

                await _emailService.SendEmailAsync(
                    pendingUser.Email,
                    "Email Verification OTP",
                    emailBody);

                return CreatedAtAction(
                    nameof(Register),
                    new { id = pendingUser.Id },
                    ApiResponse<object>.Ok(
                        new
                        {
                            pendingUser.Id,
                            pendingUser.Name,
                            pendingUser.Email,
                            pendingUser.Role
                        },
                        "Registration successful. Please check your email for the verification OTP.",
                        HttpContext.TraceIdentifier));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled registration error for email {Email}. TraceId: {TraceId}", dto?.Email, HttpContext.TraceIdentifier);

                return StatusCode(500, ApiResponse<object>.Fail(
                    "An unexpected error occurred.",
                    traceId: HttpContext.TraceIdentifier));
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

            var roleIsActive = await _context.Roles
                .AsNoTracking()
                .Where(r => r.RoleName.ToLower() == user.Role.ToLower())
                .Select(r => r.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (!roleIsActive)
            {
                return Unauthorized(ApiResponse<object>.Fail(
                "Your assigned role is inactive. Please contact the administrator.",
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

        public async Task<IActionResult> ForgotPassword(

    ForgotPasswordDto dto,

    CancellationToken cancellationToken)

        {

            var email = dto.Email.Trim().ToLowerInvariant();

            var user = await _context.Users

                .FirstOrDefaultAsync(

                    user => user.Email == email && user.IsActive,

                    cancellationToken);

            if (user != null)

            {

                // Delete old unused password-reset OTPs

                var oldOtps = await _context.Otps

                    .Where(x =>

                        x.Email == email &&

                        !x.IsUsed)

                    .ToListAsync(cancellationToken);

                _context.Otps.RemoveRange(oldOtps);

                // Generate new OTP

                var otpCode = RandomNumberGenerator

                    .GetInt32(100000, 1000000)

                    .ToString();

                // Save OTP

                var otp = new Otp

                {

                    Email = email,

                    Code = otpCode,

                    CreatedAt = DateTime.UtcNow,

                    ExpiryTime = DateTime.UtcNow.AddMinutes(5),

                    IsUsed = false,

                    Purpose = "PasswordReset"

                };

                _context.Otps.Add(otp);

                await _context.SaveChangesAsync(cancellationToken);

                // Email body

                var emailBody = $@"
<h2>Password Reset</h2>
 
<p>Hello {user.Name},</p>
 
<p>Your password reset verification code is:</p>
 
<h1 style='color:blue'>{otpCode}</h1>
 
<p>This code is valid for 5 minutes.</p>
 
<p>If you did not request a password reset, please ignore this email.</p>";

                // Send OTP email

                await _emailService.SendEmailAsync(

                    user.Email,

                    "IMS Password Reset OTP",

                    emailBody);

                _logger.LogInformation(

                    "Password reset OTP generated and sent for {Email}. TraceId: {TraceId}",

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

            if (!System.Text.RegularExpressions.Regex.IsMatch(dto.NewPassword, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\d\s]).{8,}$"))
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Password must contain at least 1 uppercase letter, 1 lowercase letter, 1 number, and 1 special character.",
                    traceId: HttpContext.TraceIdentifier));
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.TokenVersion++;

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
            var currentUserIdClaim =
                User.FindFirst("UserId")?.Value ??
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(currentUserIdClaim, out var authUserId) || authUserId <= 0)
            {
                return Unauthorized(ApiResponse<object>.Fail(
                    "User identity could not be verified.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var currentUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (authUserId != userId && !string.Equals(currentUserRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.Fail(
                    "You are not authorized to logout another user.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var targetUserId = authUserId != userId && string.Equals(currentUserRole, "Admin", StringComparison.OrdinalIgnoreCase)
                ? userId
                : authUserId;

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == targetUserId, cancellationToken);
            if (user != null)
            {
                user.TokenVersion++;
                await _refreshTokenService.RevokeAllUserTokensAsync(targetUserId);
                await _context.SaveChangesAsync(cancellationToken);
            }

            await _loginHistoryService.RecordLogoutAsync(
                targetUserId,
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
            var email = dto.Email.Trim().ToLowerInvariant();
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

            if (!System.Text.RegularExpressions.Regex.IsMatch(dto.NewPassword, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\d\s]).{8,}$"))
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Password must contain at least 1 uppercase letter, 1 lowercase letter, 1 number, and 1 special character.",
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
            try
            {
                // ============================================================
                // EMAIL BASED OTP FLOW
                // Registration verification OR Password Reset verification
                // ============================================================
                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    var email = dto.Email.Trim().ToLowerInvariant();
                    var enteredOtp = dto.Otp.Trim();

                    // First identify the OTP and its actual purpose.
                    var otp = await _context.Otps
                        .Where(x =>
                            x.Email == email &&
                            x.Code == enteredOtp &&
                            !x.IsUsed)
                        .OrderByDescending(x => x.CreatedAt)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (otp == null)
                    {
                        return BadRequest(ApiResponse<object>.Fail(
                            "Invalid OTP.",
                            traceId: HttpContext.TraceIdentifier));
                    }

                    if (otp.ExpiryTime < DateTime.UtcNow)
                    {
                        return BadRequest(ApiResponse<object>.Fail(
                            "OTP has expired.",
                            traceId: HttpContext.TraceIdentifier));
                    }

                    // ========================================================
                    // PASSWORD RESET OTP
                    // ========================================================
                    // Do NOT mark it as used here because ResetPassword()
                    // still needs the same OTP to perform the password reset.
                    if (string.Equals(
                            otp.Purpose,
                            "PasswordReset",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        var userExists = await _context.Users
                            .AsNoTracking()
                            .AnyAsync(
                                x => x.Email == email && x.IsActive,
                                cancellationToken);

                        if (!userExists)
                        {
                            return BadRequest(ApiResponse<object>.Fail(
                                "Invalid or expired verification code.",
                                traceId: HttpContext.TraceIdentifier));
                        }

                        return Ok(ApiResponse<object>.Ok(
                            null,
                            "Verification code verified successfully.",
                            HttpContext.TraceIdentifier));
                    }

                    // ========================================================
                    // REGISTRATION EMAIL VERIFICATION OTP
                    // ========================================================
                    if (string.Equals(
                            otp.Purpose,
                            "EmailVerification",
                            StringComparison.OrdinalIgnoreCase))
                    {
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

                        // Mark registration OTP as used
                        otp.IsUsed = true;

                        // Existing business logic:
                        // Convert PendingUser into User.
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
                        _context.PendingUsers.Remove(pendingUser);

                        await _context.SaveChangesAsync(cancellationToken);

                        return Ok(ApiResponse<object>.Ok(
                            null,
                            "Email verified successfully. You can now login.",
                            HttpContext.TraceIdentifier));
                    }

                    return BadRequest(ApiResponse<object>.Fail(
                        "Invalid OTP.",
                        traceId: HttpContext.TraceIdentifier));
                }

                // ============================================================
                // LOGIN 2FA OTP FLOW
                // Existing business logic remains unchanged
                // ============================================================

                if (!dto.UserId.HasValue || dto.UserId.Value <= 0)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        "Email address or user ID is required.",
                        traceId: HttpContext.TraceIdentifier));
                }

                var loginOtp = await _context.Otps
                    .Where(x =>
                        x.UserId == dto.UserId.Value &&
                        x.Code == dto.Otp.Trim() &&
                        x.Purpose == "Login" &&
                        !x.IsUsed)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync(cancellationToken);

                if (loginOtp == null)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        "Invalid OTP.",
                        traceId: HttpContext.TraceIdentifier));
                }

                if (loginOtp.ExpiryTime < DateTime.UtcNow)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        "OTP has expired.",
                        traceId: HttpContext.TraceIdentifier));
                }

                loginOtp.IsUsed = true;

                var loginUser = await _context.Users
                    .FirstOrDefaultAsync(
                        x => x.Id == dto.UserId.Value,
                        cancellationToken);

                if (loginUser == null)
                {
                    return BadRequest(ApiResponse<object>.Fail(
                        "User not found.",
                        traceId: HttpContext.TraceIdentifier));
                }

                var permissions =
                    await _permissionService.GetPermissionsAsync(loginUser.Role);

                await _loginHistoryService.RecordLoginAsync(
                    loginUser.Id,
                    Request.Headers["User-Agent"].ToString(),
                    HttpContext.Connection.RemoteIpAddress?.ToString());

                var accessToken =
                    _jwtService.GenerateAccessToken(loginUser);

                var refreshToken =
                    _jwtService.GenerateRefreshToken(
                        loginUser.Id,
                        HttpContext.Connection.RemoteIpAddress?.ToString()
                            ?? "Unknown",
                        Request.Headers["User-Agent"].ToString());

                await _refreshTokenService
                    .SaveRefreshTokenAsync(refreshToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Ok(ApiResponse<object>.Ok(
                    new
                    {
                        token = accessToken,
                        refreshToken = refreshToken.Token,
                        expiresAt = DateTime.UtcNow.AddHours(8),

                        user = new
                        {
                            loginUser.Id,
                            loginUser.Name,
                            loginUser.Email,
                            loginUser.Role
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
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception in VerifyOtp. TraceId: {TraceId}",
                    HttpContext.TraceIdentifier);

                return StatusCode(
                    500,
                    ApiResponse<object>.Fail(
                        "An unexpected error occurred.",
                        traceId: HttpContext.TraceIdentifier));
            }
        }

        [HttpPost("verify-email-otp")]
        public async Task<IActionResult> VerifyEmailOtp(
            VerifyEmailOtpDto dto,
            CancellationToken cancellationToken)
        {
            return await VerifyOtp(
                new VerifyOtpDto { Email = dto.Email, Otp = dto.Otp },
                cancellationToken);
        }


        [HttpPost("resend-verification")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendVerificationEmail(
            ResendVerificationDto dto,
            CancellationToken cancellationToken)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest(ApiResponse<object>.Fail("Email is required.", traceId: HttpContext.TraceIdentifier));
            }

            var email = dto.Email.Trim().ToLowerInvariant();

            var pendingUser = await _context.PendingUsers
                .FirstOrDefaultAsync(
                    x => x.Email == email,
                    cancellationToken);

            var existingUser = pendingUser == null
                ? await _context.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken)
                : null;

            if (pendingUser == null && existingUser == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "No account matches that email.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var recipientName = pendingUser?.Name ?? existingUser?.Name ?? "User";

            // Delete old unused email verification OTPs
            var oldOtps = await _context.Otps
                .Where(x =>
                    x.Email == email &&
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

        <p>Hello {recipientName},</p>

        <p>Your email verification OTP is:</p>

        <h1>{otpCode}</h1>

        <p>This OTP is valid for 10 minutes.</p>

        <p>If you did not request this, please ignore this email.</p>";

            await _emailService.SendEmailAsync(
                email,
                "Email Verification OTP",
                emailBody);

            return Ok(ApiResponse<object>.Ok(
                null,
                "Verification OTP sent successfully.",
                HttpContext.TraceIdentifier));
        }

        [HttpPost("resend-login-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendLoginOtp(
            ResendLoginOtpDto dto,
            CancellationToken cancellationToken)
        {
            if (dto == null || (string.IsNullOrWhiteSpace(dto.Email) && (dto.UserId == null || dto.UserId <= 0)))
            {
                return BadRequest(ApiResponse<object>.Fail("Email or User ID is required.", traceId: HttpContext.TraceIdentifier));
            }

            User? user = null;
            PendingUser? pendingUser = null;

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var email = dto.Email.Trim().ToLowerInvariant();
                user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
                if (user == null)
                {
                    pendingUser = await _context.PendingUsers.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
                }
            }
            else if (dto.UserId.HasValue)
            {
                user = await _context.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId.Value, cancellationToken);
            }

            if (user == null && pendingUser == null)
            {
                return NotFound(ApiResponse<object>.Fail("No account matches that information.", traceId: HttpContext.TraceIdentifier));
            }

            var targetEmail = user?.Email ?? pendingUser?.Email;
            var recipientName = user?.Name ?? pendingUser?.Name ?? "User";

            if (string.IsNullOrWhiteSpace(targetEmail))
            {
                return BadRequest(ApiResponse<object>.Fail("Email address is missing.", traceId: HttpContext.TraceIdentifier));
            }

            // Remove old unused OTPs
            var oldOtps = await _context.Otps
                .Where(x => x.Email == targetEmail && !x.IsUsed)
                .ToListAsync(cancellationToken);
            _context.Otps.RemoveRange(oldOtps);

            var otpCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var otp = new Otp
            {
                Email = targetEmail,
                Code = otpCode,
                CreatedAt = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                Purpose = "Login"
            };

            _context.Otps.Add(otp);
            await _context.SaveChangesAsync(cancellationToken);

            var emailBody = $@"
        <h2>Login Verification Code</h2>
        <p>Hello {recipientName},</p>
        <p>Your verification code is:</p>
        <h1 style='color:blue'>{otpCode}</h1>
        <p>This code is valid for 10 minutes.</p>";

            await _emailService.SendEmailAsync(targetEmail, "Login Verification Code", emailBody);

            return Ok(ApiResponse<object>.Ok(null, "Verification code resent successfully.", HttpContext.TraceIdentifier));
        }



    }



}







