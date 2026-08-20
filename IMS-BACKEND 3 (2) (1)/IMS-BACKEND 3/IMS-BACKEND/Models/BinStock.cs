using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("bin_stock")]
    public class BinStock
    {
        [Key]
        [Column("bin_stock_id")]
        public int BinStockId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Column("warehouse_id")]
        public int WarehouseId { get; set; }

        [Column("bin_id")]
        public int BinId { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }
    }
}