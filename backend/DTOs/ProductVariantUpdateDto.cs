public class ProductVariantUpdateDto
{
    public string VariantName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public decimal? CostPrice { get; set; }
}