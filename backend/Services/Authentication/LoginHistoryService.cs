using IMSBackend.Data;
using IMSBackend.Interfaces;
using IMSBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Services.Authentication
{
    public class LoginHistoryService : ILoginHistoryService
    {
        private readonly AppDbContext _context;

        public LoginHistoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task RecordLoginAsync(
            int userId,
            string? userAgent,
            string? ipAddress)
        {
            var loginHistory = new LoginHistory
            {
                UserId = userId,
                LoginTime = DateTime.UtcNow,
                DeviceInfo = userAgent,
                Browser = GetBrowser(userAgent),
                OperatingSystem = GetOperatingSystem(userAgent),
                IpAddress = ipAddress,
                IsCurrentSession = true
            };

            _context.LoginHistories.Add(loginHistory);

            await _context.SaveChangesAsync();
        }

        public async Task RecordLogoutAsync(
            int userId,
            string logoutType)
        {
            var session = await _context.LoginHistories
                .Where(x => x.UserId == userId &&
                            x.IsCurrentSession)
                .OrderByDescending(x => x.LoginTime)
                .FirstOrDefaultAsync();

            if (session == null)
                return;

            session.LogoutTime = DateTime.UtcNow;
            session.LogoutType = logoutType;
            session.IsCurrentSession = false;

            await _context.SaveChangesAsync();
        }


        public async Task RecordLogoutAllAsync(
    int userId,
    string logoutType)
        {
            var sessions = await _context.LoginHistories
                .Where(x => x.UserId == userId &&
                            x.IsCurrentSession)
                .ToListAsync();

            foreach (var session in sessions)
            {
                session.LogoutTime = DateTime.UtcNow;
                session.LogoutType = logoutType;
                session.IsCurrentSession = false;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<LoginHistory>> GetUserHistoryAsync(
            int userId)
        {
            return await _context.LoginHistories
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.LoginTime)
                .ToListAsync();
        }

        private static string GetBrowser(string? userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                return "Unknown";

            if (userAgent.Contains("Edg"))
                return "Microsoft Edge";

            if (userAgent.Contains("Chrome"))
                return "Google Chrome";

            if (userAgent.Contains("Firefox"))
                return "Mozilla Firefox";

            if (userAgent.Contains("Safari"))
                return "Safari";

            return "Unknown";
        }

        private static string GetOperatingSystem(string? userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                return "Unknown";

            if (userAgent.Contains("Windows"))
                return "Windows";

            if (userAgent.Contains("Android"))
                return "Android";

            if (userAgent.Contains("iPhone"))
                return "iPhone";

            if (userAgent.Contains("Mac"))
                return "MacOS";

            if (userAgent.Contains("Linux"))
                return "Linux";

            return "Unknown";
        }
    }
}