using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("sales_return_items")]
    public class SalesReturnItem
    {
        [Key]
        [Column("SalesReturnItemId")]
        public int SalesReturnItemId { get; set; }

        [Column("SalesReturnId")]
        public int SalesReturnId { get; set; }

        [ForeignKey("SalesReturnId")]
        public SalesReturn? SalesReturn { get; set; }

        [Column("ProductId")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [Column("VariantId")]
        public int? VariantId { get; set; }

        [ForeignKey("VariantId")]
        public ProductVariant? Variant { get; set; }

        [Column("InvoicedQuantity", TypeName = "decimal(18,3)")]
        public decimal InvoicedQuantity { get; set; }

        [Column("ReturnQuantity", TypeName = "decimal(18,3)")]
        public decimal ReturnQuantity { get; set; }

        [Column("Price", TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column("Total", TypeName = "decimal(18,2)")]
        public decimal Total { get; set; } = 0.00m;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
