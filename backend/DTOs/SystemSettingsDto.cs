using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class SystemSettingsDto
    {
        [Required]
        [StringLength(255)]
        public string? CompanyName { get; set; }

        [Required]
        [EmailAddress]
        public string? CompanyEmail { get; set; }

        [StringLength(50)]
        public string? CompanyPhone { get; set; }

        public string? CompanyAddress { get; set; }

        public string? CompanyLogo { get; set; }

        [StringLength(100)]
        public string? GstNumber { get; set; }

        [Required]
        public string? Currency { get; set; }

        [Required]
        public string? Timezone { get; set; }

        public bool AllowNegativeStock { get; set; }

        [Range(0, 10000)]
        public int DefaultReorderLevel { get; set; }

        public string? StockValuationMethod { get; set; }

        [Required]
        [StringLength(20)]
        public string? InvoicePrefix { get; set; }

        [Range(1, 999999)]
        public int InvoiceStartNumber { get; set; }

        public bool EnableAuditLogs { get; set; }

        [Range(30, 3650)]
        public int AuditRetentionDays { get; set; }

        public bool LowStockAlert { get; set; }

        public string? DefaultUnitType { get; set; }

        public bool EnableBarcode { get; set; }

        public bool AutoStockUpdate { get; set; }


        public bool EmailNotifications { get; set; }

        public bool LowStockNotifications { get; set; }

        public bool PurchaseNotifications { get; set; }

        public bool SalesNotifications { get; set; }

        public bool SystemAlerts { get; set; }

        public bool EnableTwoFactorAuth { get; set; }

        public string? ThemeMode { get; set; }

        public string? Language { get; set; }

        public bool CollapseSidebar { get; set; }
    }
}