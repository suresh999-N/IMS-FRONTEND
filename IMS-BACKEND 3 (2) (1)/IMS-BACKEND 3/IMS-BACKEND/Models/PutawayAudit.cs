using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("putaway_audits")]
    public class PutawayAudit
    {
        [Key]
        [Column("putaway_audit_id")]
        public int PutawayAuditId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Column("warehouse_id")]
        public int WarehouseId { get; set; }

        [Column("rack_id")]
        public int RackId { get; set; }

        [Column("bin_id")]
        public int BinId { get; set; }

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
