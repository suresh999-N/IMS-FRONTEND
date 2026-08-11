using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("invoice_items")]
    public class InvoiceItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("invoice_id")]
        public int? InvoiceId { get; set; }

        [Column("product_id")]
        public int? ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("tax_percent")]
        public decimal TaxPercent { get; set; }

        [Column("tax_amount")]
        public decimal TaxAmount { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }
}