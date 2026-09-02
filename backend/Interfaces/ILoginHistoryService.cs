using IMSBackend.Models;

namespace IMSBackend.Interfaces
{
    public interface ILoginHistoryService
    {
        Task RecordLoginAsync(
            int userId,
            string? userAgent,
            string? ipAddress);

        Task RecordLogoutAsync(
            int userId,
            string logoutType);

        // NEW
        Task RecordLogoutAllAsync(
            int userId,
            string logoutType);

        Task<List<LoginHistory>> GetUserHistoryAsync(
            int userId);
    }
}