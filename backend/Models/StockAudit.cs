using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("stock_audits")]
    public class StockAudit
    {
        [Key]
        [Column("audit_id")]
        public int AuditId { get; set; }

        [Column("warehouse_id")]
        public int WarehouseId { get; set; }

        [Column("audit_date")]
        public DateTime AuditDate { get; set; }

        [Column("audit_type")]
        public string? AuditType { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("created_by")]
        public int? CreatedBy { get; set; }

        [Column("approved_by")]
        public int? ApprovedBy { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }
    }
}