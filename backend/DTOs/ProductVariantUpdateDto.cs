using System.ComponentModel.DataAnnotations;

public class ProductVariantUpdateDto
{
    [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name can contain only letters and spaces.")]
    public string VariantName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public decimal? CostPrice { get; set; }
}