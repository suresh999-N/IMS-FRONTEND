using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("sales_return_items")]
    public class SalesReturnItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("sales_return_id")]
        public int SalesReturnId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Column("invoiced_quantity", TypeName = "decimal(18,3)")]
        public decimal InvoicedQuantity { get; set; }

        [Column("return_quantity", TypeName = "decimal(18,3)")]
        public decimal ReturnQuantity { get; set; }

        [Column("price", TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column("tax", TypeName = "decimal(18,2)")]
        public decimal Tax { get; set; }

        [Column("tax_amount", TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column("discount", TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        [Column("total", TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(SalesReturnId))]
        public SalesReturn? SalesReturn { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        [ForeignKey(nameof(VariantId))]
        public ProductVariant? Variant { get; set; }
    }
}