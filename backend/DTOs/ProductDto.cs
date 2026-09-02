using System.ComponentModel.DataAnnotations;

public class ProductDto
{
    [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name can contain only letters and spaces.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "SKU is required.")]
    [StringLength(50, MinimumLength = 6, ErrorMessage = "SKU must contain at least 6 characters.")]
    [RegularExpression(@"^[A-Za-z0-9_-]+$", ErrorMessage = "SKU can contain only letters, numbers, hyphens, and underscores.")]
    public string SKU { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;

    public int? CategoryId { get; set; }
    public int? SubCategoryId { get; set; }
    public int? BrandId { get; set; }
    public int? UnitId { get; set; }

    public decimal? Price { get; set; }
    public decimal? CostPrice { get; set; }

    public int? Stock { get; set; }
    public int? ReorderLevel { get; set; }

    public int? SupplierId { get; set; }
    public int? WarehouseId { get; set; }

    public string Status { get; set; } = "active";
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }

    // 🔥 IMPORTANT
    public List<ProductVariantCreateDto>? Variants { get; set; }
}
