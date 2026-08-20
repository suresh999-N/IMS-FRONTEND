using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("stock_movements")]
    public class StockMovement
    {
        [Key]
        [Column("movement_id")]
        public int MovementId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Column("warehouse_id")]
        public int WarehouseId { get; set; }

        [Column("movement_type")]
        public string? MovementType { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [Column("reference_id")]
        public int? ReferenceId { get; set; }

        [Column("reference_type")]
        public string? ReferenceType { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

    }
}
