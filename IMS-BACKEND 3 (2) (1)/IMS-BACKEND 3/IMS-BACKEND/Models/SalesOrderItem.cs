using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("sales_order_items")]
    public class SalesOrderItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("so_id")]
        public int? SoId { get; set; }

        [Column("product_id")]
        public int? ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [Column("delivered_quantity")]
        public decimal DeliveredQuantity { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("total")]
        public decimal Total { get; set; }
    }
}