using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("stock_audit_items")]
    public class StockAuditItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("audit_id")]
        public int? AuditId { get; set; }

        [Column("product_id")]
        public int? ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Column("bin_id")]
        public int? BinId { get; set; }

        [Column("system_quantity")]
        public decimal? SystemQuantity { get; set; }

        [Column("physical_quantity")]
        public decimal? PhysicalQuantity { get; set; }

        [Column("difference")]
        public decimal? Difference { get; set; }
    }
}