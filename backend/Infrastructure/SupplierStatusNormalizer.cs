namespace IMSBackend.Infrastructure
{
    public static class SupplierStatusNormalizer
    {
        private static readonly HashSet<string> ValidStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            "active",
            "blocked",
            "inactive",
            "pending"
        };

        public static string? Normalize(string? status, string? fallback = null)
        {
            var rawStatus = string.IsNullOrWhiteSpace(status) ? fallback : status;
            var normalizedStatus = rawStatus?.Trim().ToLowerInvariant();

            return normalizedStatus != null && ValidStatuses.Contains(normalizedStatus)
                ? normalizedStatus
                : null;
        }
    }
}
