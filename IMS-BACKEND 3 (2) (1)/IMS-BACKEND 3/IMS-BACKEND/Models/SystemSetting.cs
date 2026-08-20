using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("system_settings")]
    public class SystemSetting
    {
        [Key]
        [Column("setting_id")]
        public int SettingId { get; set; }

        [Column("company_name")]
        public string? CompanyName { get; set; }

        [Column("company_email")]
        public string? CompanyEmail { get; set; }

        [Column("company_phone")]
        public string? CompanyPhone { get; set; }

        [Column("company_address")]
        public string? CompanyAddress { get; set; }

        [Column("company_logo")]
        public string? CompanyLogo { get; set; }

        [Column("gst_number")]
        public string? GstNumber { get; set; }

        [Column("currency")]
        public string? Currency { get; set; }

        [Column("timezone")]
        public string? Timezone { get; set; }

        [Column("invoice_prefix")]
        public string? InvoicePrefix { get; set; }

        [Column("allow_negative_stock")]
        public bool AllowNegativeStock { get; set; }

        [Column("default_reorder_level")]
        public int DefaultReorderLevel { get; set; }

        [Column("stock_valuation_method")]
        public string? StockValuationMethod { get; set; }

        [Column("invoice_start_number")]
        public int InvoiceStartNumber { get; set; }

        [Column("enable_audit_logs")]
        public bool EnableAuditLogs { get; set; }

        [Column("audit_retention_days")]
        public int AuditRetentionDays { get; set; }

        [Column("low_stock_alert")]
        public bool LowStockAlert { get; set; }


        [Column("default_unit_type")]
        public string? DefaultUnitType { get; set; }

        [Column("enable_barcode")]
        public bool EnableBarcode { get; set; }

        [Column("auto_stock_update")]
        public bool AutoStockUpdate { get; set; }

        [Column("email_notifications")]
        public bool EmailNotifications { get; set; }

        [Column("low_stock_notifications")]
        public bool LowStockNotifications { get; set; }

        [Column("purchase_notifications")]
        public bool PurchaseNotifications { get; set; }

        [Column("sales_notifications")]
        public bool SalesNotifications { get; set; }

        [Column("system_alerts")]
        public bool SystemAlerts { get; set; }

        [Column("enable_two_factor_auth")]
        public bool EnableTwoFactorAuth { get; set; }


        [Column("theme_mode")]
        public string? ThemeMode { get; set; }

        [Column("language")]
        public string? Language { get; set; }

        [Column("collapse_sidebar")]
        public bool CollapseSidebar { get; set; }


        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}