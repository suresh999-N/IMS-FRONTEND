using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("bin_transfer_audits")]
    public class BinTransferAudit
    {
        [Key]
        [Column("bin_transfer_audit_id")]
        public int BinTransferAuditId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Column("warehouse_id")]
        public int WarehouseId { get; set; }

        [Column("from_bin_id")]
        public int FromBinId { get; set; }

        [Column("to_bin_id")]
        public int ToBinId { get; set; }

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
