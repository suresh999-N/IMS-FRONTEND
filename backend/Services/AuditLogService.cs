using System.Security.Claims;
using System.Text.RegularExpressions;
using IMSBackend.Data;
using IMSBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Services
{
    public class AuditLogService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(
            string action,
            string module,
            int? recordId,
            string description,
            string tableName,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var userName = await GetCurrentUserNameAsync(userId, cancellationToken);

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = action,
                Module = module,
                RecordId = recordId,
                Description = BuildDescription(userName, description),
                TableName = tableName,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync(cancellationToken);
        }

        private int? GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var value =
                user?.FindFirstValue(ClaimTypes.NameIdentifier) ??
                user?.FindFirstValue("userId") ??
                user?.FindFirstValue("sub");

            return int.TryParse(value, out var userId) ? userId : null;
        }

        private async Task<string?> GetCurrentUserNameAsync(int? userId, CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var claimName =
                user?.FindFirstValue("name") ??
                user?.FindFirstValue(ClaimTypes.GivenName) ??
                user?.FindFirstValue(ClaimTypes.Name);

            if (!string.IsNullOrWhiteSpace(claimName) && !claimName.Contains('@'))
            {
                return claimName.Trim();
            }

            if (userId is null)
            {
                return string.IsNullOrWhiteSpace(claimName) ? null : claimName.Trim();
            }

            var account = await _context.Users
                .AsNoTracking()
                .Where(item => item.Id == userId.Value)
                .Select(item => new { item.Name, item.Email })
                .FirstOrDefaultAsync(cancellationToken);

            return !string.IsNullOrWhiteSpace(account?.Name)
                ? account.Name.Trim()
                : account?.Email?.Trim() ?? claimName?.Trim();
        }

        private static string BuildDescription(string? userName, string description)
        {
            var cleanDescription = string.IsNullOrWhiteSpace(description)
                ? "activity recorded"
                : description.Trim();

            if (string.IsNullOrWhiteSpace(userName) ||
                cleanDescription.StartsWith(userName, StringComparison.OrdinalIgnoreCase))
            {
                return cleanDescription;
            }

            return $"{userName.Trim()} {ToActorPhrase(cleanDescription)}";
        }

        private static string ToActorPhrase(string description)
        {
            var entityMatch = Regex.Match(
                description,
                @"^(Product|Customer|Supplier|Category|Sub Category|Brand|Invoice|Purchase Order|Goods Receipt)\s+(.+?)\s+(created|updated|deleted)$",
                RegexOptions.IgnoreCase);

            if (entityMatch.Success)
            {
                var name = entityMatch.Groups[2].Value.Trim();
                var action = entityMatch.Groups[3].Value.Trim().ToLowerInvariant();
                return $"{action} {name}";
            }

            var statusMatch = Regex.Match(
                description,
                @"^(Customer|Supplier)\s+status\s+changed\s+from\s+(.+?)\s+to\s+(.+)$",
                RegexOptions.IgnoreCase);

            if (statusMatch.Success)
            {
                var entity = statusMatch.Groups[1].Value.Trim().ToLowerInvariant();
                var nextStatus = statusMatch.Groups[3].Value.Trim().ToLowerInvariant();

                if (nextStatus == "active")
                {
                    return $"activated {entity}";
                }

                if (nextStatus == "inactive")
                {
                    return $"deactivated {entity}";
                }

                return $"updated {entity} status";
            }

            return description;
        }
    }
}
