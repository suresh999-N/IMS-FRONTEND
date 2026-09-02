using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace IMSBackend.Models
{
    [Table("purchase_return_items")]
    public class PurchaseReturnItem
    {
        [Key]
        [Column("purchase_return_item_id")]
        public int PurchaseReturnItemId { get; set; }

        [Column("purchase_return_id")]
        public int PurchaseReturnId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Column("received_quantity", TypeName = "decimal(18,3)")]
        public decimal ReceivedQuantity { get; set; }

        [Column("return_quantity", TypeName = "decimal(18,3)")]
        public decimal ReturnQuantity { get; set; }

        [Column("price", TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column("total", TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey(nameof(PurchaseReturnId))]
        [JsonIgnore]
        public virtual PurchaseReturn? PurchaseReturn { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product? Product { get; set; }

        [ForeignKey(nameof(VariantId))]
        public virtual ProductVariant? ProductVariant { get; set; }
    }
}