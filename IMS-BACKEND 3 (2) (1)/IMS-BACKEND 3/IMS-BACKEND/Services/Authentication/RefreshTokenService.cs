using IMSBackend.Data;
using IMSBackend.Interfaces;
using IMSBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Services.Authentication
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly AppDbContext _context;

        public RefreshTokenService(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // SAVE REFRESH TOKEN
        // =========================
        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        // =========================
        // GET REFRESH TOKEN
        // =========================
        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == token);
        }

        // =========================
        // REVOKE SINGLE TOKEN
        // =========================
        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == token);

            if (refreshToken == null)
                return;

            refreshToken.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // =========================
        // REVOKE ALL USER TOKENS
        // =========================
        public async Task RevokeAllUserTokensAsync(int userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(x => x.UserId == userId && x.RevokedAt == null)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        // =========================
        // UPDATE REFRESH TOKEN
        // =========================
        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);

            await _context.SaveChangesAsync();
        }

        // =========================
        // DELETE EXPIRED TOKENS
        // =========================
        public async Task DeleteExpiredTokensAsync()
        {
            var expiredTokens = await _context.RefreshTokens
                .Where(x => x.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            if (!expiredTokens.Any())
                return;

            _context.RefreshTokens.RemoveRange(expiredTokens);

            await _context.SaveChangesAsync();
        }
    }
}