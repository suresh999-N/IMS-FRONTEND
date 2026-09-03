using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }

        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name can contain only letters and spaces.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "SKU is required.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "SKU must contain at least 6 characters.")]
        [RegularExpression(@"^[A-Za-z0-9_-]+$", ErrorMessage = "SKU can contain only letters, numbers, hyphens, and underscores.")]
        public string SKU { get; set; } = string.Empty;

        [Column("category_id")]
        public int? CategoryId { get; set; }

        [Column("sub_category_id")]
        public int? SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public SubCategory? SubCategory { get; set; }

        [Column("brand_id")]
        public int? BrandId { get; set; }

        [Column("unit_id")]
        public int? UnitId { get; set; }

        public decimal? Price { get; set; }

        [Column("cost_price")]
        public decimal? CostPrice { get; set; }

        public string Barcode { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = "active";

        [Column("reorder_level")]
        public int? ReorderLevel { get; set; }

        [Column("supplier_id")]
        public int? SupplierId { get; set; }

        [Column("warehouse_id")]
        public int? WarehouseId { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        [Column("is_archived")]
        public bool IsArchived { get; set; } = false;

        [Column("image_url")]
        public string? ImageUrl { get; set; }

        public ICollection<InvoiceItem>? InvoiceItems { get; set; }

        public ICollection<PurchaseIndentItem> PurchaseIndentItems { get; set; }
            = new List<PurchaseIndentItem>();
    }
}
