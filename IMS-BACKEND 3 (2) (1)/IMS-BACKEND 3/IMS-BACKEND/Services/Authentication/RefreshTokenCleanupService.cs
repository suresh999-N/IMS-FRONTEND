using IMSBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Services.Authentication
{
    public class RefreshTokenCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RefreshTokenCleanupService> _logger;

        public RefreshTokenCleanupService(
            IServiceScopeFactory scopeFactory,
            ILogger<RefreshTokenCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Refresh Token Cleanup Service Started.");

            using var timer = new PeriodicTimer(TimeSpan.FromHours(1));

            // Perform initial cleanup when background service starts
            await CleanExpiredTokensAsync(stoppingToken);

            try
            {
                while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await CleanExpiredTokensAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Graceful cancellation on app shutdown
            }

            _logger.LogInformation("Refresh Token Cleanup Service Stopped.");
        }

        private async Task CleanExpiredTokensAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var expiredTokens = await context.RefreshTokens
                    .Where(x => x.ExpiresAt < DateTime.UtcNow || x.RevokedAt != null)
                    .ToListAsync(cancellationToken);

                if (expiredTokens.Any())
                {
                    context.RefreshTokens.RemoveRange(expiredTokens);
                    await context.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation("{Count} expired refresh tokens deleted.", expiredTokens.Count);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Ignore cancellation during shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while cleaning refresh tokens.");
            }
        }
    }
}