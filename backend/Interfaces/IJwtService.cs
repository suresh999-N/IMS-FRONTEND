using IMSBackend.Models;

namespace IMSBackend.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);

        RefreshToken GenerateRefreshToken(
            int userId,
            string ipAddress,
            string? deviceName);
    }
}