using IMSBackend.Models;

namespace IMSBackend.Interfaces
{
    public interface IRefreshTokenService
    {
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);

        Task<RefreshToken?> GetRefreshTokenAsync(string token);

        Task RevokeRefreshTokenAsync(string token);

        Task RevokeAllUserTokensAsync(int userId);

        // NEW
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);

        // NEW
        Task DeleteExpiredTokensAsync();
    }
}