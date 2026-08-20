public class ProductVariantCreateDto
{
    public string VariantName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;

    public decimal? PriceDelta { get; set; }
    public int? StockDelta { get; set; }

    public List<VariantAttributeValueDto>? Attributes { get; set; }
}