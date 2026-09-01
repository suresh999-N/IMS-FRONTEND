using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("product_variants")]
    public class ProductVariant
    {
        [Key]   // 🔥 IMPORTANT
        [Column("variant_id")]
        public int VariantId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_name")]
        public string VariantName { get; set; } = string.Empty;

        [Column("sku")]
        public string SKU { get; set; } = string.Empty;

        [Column("price")]
        public decimal? Price { get; set; }

        [Column("cost_price")]
        public decimal? CostPrice { get; set; }
    }
}