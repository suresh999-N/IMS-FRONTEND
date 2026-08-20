using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("purchase_order_items")]
    public class PurchaseOrderItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("po_id")]
        public int? PoId { get; set; }

        [Column("product_id")]
        public int? ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Column("quantity")]
        public decimal? Quantity { get; set; }

        [Column("received_quantity")]
        public decimal? ReceivedQuantity { get; set; }

        [Column("price")]
        public decimal? Price { get; set; }

        [Column("discount")]
        public decimal? Discount { get; set; }

        [Column("tax")]
        public decimal? Tax { get; set; }

        [Column("total")]
        public decimal? Total { get; set; }
    }
}