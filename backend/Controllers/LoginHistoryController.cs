using IMSBackend.Data;
using IMSBackend.Models;
using IMSBackend.DTOs;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginHistoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoginHistoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/LoginHistory/1
        // GET: api/LoginHistory/1
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetLoginHistory(int userId)
        {
            var history = await _context.LoginHistories
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.LoginTime)
                .Select(x => new LoginHistoryDto
                {
                    LoginTime = x.LoginTime,
                    LogoutTime = x.LogoutTime,
                    Browser = x.Browser,
                    OperatingSystem = x.OperatingSystem,
                    DeviceInfo = x.DeviceInfo,
                    IpAddress = x.IpAddress,
                    LogoutType = x.LogoutType,
                    IsCurrentSession = x.IsCurrentSession
                })
                .ToListAsync();

            return Ok(history);
        }



        [HttpGet("current-session/{userId}")]
        public async Task<IActionResult> GetCurrentSession(int userId)
        {
            var session = await _context.LoginHistories
                .Where(x => x.UserId == userId && x.IsCurrentSession)
                .OrderByDescending(x => x.LoginTime)
                .Select(x => new LoginHistoryDto
                {
                    LoginTime = x.LoginTime,
                    LogoutTime = x.LogoutTime,
                    Browser = x.Browser,
                    OperatingSystem = x.OperatingSystem,
                    DeviceInfo = x.DeviceInfo,
                    IpAddress = x.IpAddress,
                    LogoutType = x.LogoutType,
                    IsCurrentSession = x.IsCurrentSession
                })
                .FirstOrDefaultAsync();

            if (session == null)
            {
                return NotFound(new
                {
                    message = "No active session found."
                });
            }

            return Ok(session);
        }


    }
}