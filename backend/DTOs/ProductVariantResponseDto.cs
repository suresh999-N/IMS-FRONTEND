public class ProductVariantResponseDto
{
    public int VariantId { get; set; }
    public int ProductId { get; set; }
    public string VariantName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public decimal? CostPrice { get; set; }
}