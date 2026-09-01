using System.ComponentModel.DataAnnotations;

public class ProductVariantCreateDto
{
    [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name can contain only letters and spaces.")]
    public string VariantName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;

    public decimal? PriceDelta { get; set; }
    public int? StockDelta { get; set; }

    public List<VariantAttributeValueDto>? Attributes { get; set; }
}