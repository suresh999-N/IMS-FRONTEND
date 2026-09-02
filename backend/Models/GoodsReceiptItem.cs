using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("goods_receipt_items")]
    public class GoodsReceiptItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("grn_id")]
        public int? GrnId { get; set; }

        [Column("product_id")]
        public int? ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Column("quantity_received")]
        public decimal? QuantityReceived { get; set; }

        [Column("price")]
        public decimal? Price { get; set; }

        [Column("discount")]
        public decimal? Discount { get; set; }

        [Column("tax")]
        public decimal? Tax { get; set; }

        
        [Column("tax_percentage")]
        public decimal? TaxPercentage { get; set; }

        [Column("tax_amount")]
        public decimal? TaxAmount { get; set; }

        [Column("taxable_amount")]
        public decimal? TaxableAmount { get; set; }

       
        [Column("line_total")]
        public decimal? LineTotal { get; set; }
    }
}