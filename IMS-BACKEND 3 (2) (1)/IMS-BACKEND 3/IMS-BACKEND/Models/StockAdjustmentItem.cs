using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("stock_adjustment_items")]
    public class StockAdjustmentItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("adjustment_id")]
        public int AdjustmentId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }
    }
}