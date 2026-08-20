namespace IMSBackend.Infrastructure
{
    public static class SupplierDocumentTypes
    {
        public const string Gst = "GST";
        public const string Pan = "PAN";
        public const string Agreement = "AGREEMENT";
        public const string Other = "OTHER";

        private static readonly HashSet<string> ValidTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            Gst,
            Pan,
            Agreement,
            Other
        };

        public static string? Normalize(string? documentType)
        {
            var normalizedType = documentType?.Trim().ToUpperInvariant();

            return normalizedType != null && ValidTypes.Contains(normalizedType)
                ? normalizedType
                : null;
        }
    }
}
