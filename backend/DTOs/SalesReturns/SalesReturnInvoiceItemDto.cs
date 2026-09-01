namespace IMSBackend.DTOs.SalesReturns
{
    public class SalesReturnInvoiceItemDto
    {
        public int InvoiceItemId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? VariantId { get; set; }
        public string? VariantName { get; set; }
        public decimal InvoicedQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal PreviousReturnedQuantity { get; set; }
        public decimal RemainingReturnableQuantity { get; set; }
    }
}
