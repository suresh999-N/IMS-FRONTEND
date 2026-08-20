using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("stock_adjustments")]
    public class StockAdjustment
    {
        [Key]
        [Column("adjustment_id")]
        public int AdjustmentId { get; set; }

        [Column("warehouse_id")]
        public int WarehouseId { get; set; }

        [Column("adjustment_type")]
        public string? AdjustmentType { get; set; }

        [Column("reason")]
        public string? Reason { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}