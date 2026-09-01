using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("warehouse_transfer_audits")]
    public class WarehouseTransferAudit
    {
        [Key]
        [Column("warehouse_transfer_audit_id")]
        public int WarehouseTransferAuditId { get; set; }

        [Column("transfer_id")]
        public int TransferId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Column("from_warehouse_id")]
        public int FromWarehouseId { get; set; }

        [Column("to_warehouse_id")]
        public int ToWarehouseId { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [Column("user_id")]
        public int? UserId { get; set; }

        [Column("user_name")]
        public string? UserName { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
