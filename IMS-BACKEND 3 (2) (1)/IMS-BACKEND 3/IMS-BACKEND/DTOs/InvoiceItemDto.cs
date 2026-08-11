public class InvoiceItemDto
{
    public int ProductId { get; set; }

    public int? VariantId { get; set; }

    public decimal Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal TaxPercent { get; set; }

    public decimal TaxAmount { get; set; }
}