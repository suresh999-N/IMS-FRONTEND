using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("purchase_return_items")]
    public class PurchaseReturnItem
    {
        [Key]
        [Column("item_id")]
        public int PurchaseReturnItemId { get; set; }

        [Column("return_id")]
        public int PurchaseReturnId { get; set; }

        [ForeignKey("PurchaseReturnId")]
        public PurchaseReturn? PurchaseReturn { get; set; }

        [Column("product_id")]
        public int? ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [ForeignKey("VariantId")]
        public ProductVariant? Variant { get; set; }

        [Column("received_qty", TypeName = "decimal(18,2)")]
        public decimal ReceivedQuantity { get; set; }

        [Column("return_qty", TypeName = "decimal(18,2)")]
        public decimal ReturnQuantity { get; set; }

        [Column("unit_cost", TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
