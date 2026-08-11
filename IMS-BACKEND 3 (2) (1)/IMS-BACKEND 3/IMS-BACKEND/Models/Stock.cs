using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("stock")]
    public class Stock
    {
        [Key]
        [Column("stock_id")]
        public int StockId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Column("warehouse_id")]
        public int WarehouseId { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [Column("reserved_quantity")]
        public decimal ReservedQuantity { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column("available_quantity")]
        public decimal AvailableQuantity { get; set; }
    }
}