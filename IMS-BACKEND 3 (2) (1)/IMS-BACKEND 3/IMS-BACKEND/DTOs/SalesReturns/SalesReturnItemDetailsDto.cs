namespace IMSBackend.DTOs.SalesReturns
{
    public class SalesReturnItemDetailsDto
    {
        public int SalesReturnItemId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? VariantId { get; set; }
        public string? VariantName { get; set; }
        public decimal InvoicedQuantity { get; set; }
        public decimal ReturnQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
