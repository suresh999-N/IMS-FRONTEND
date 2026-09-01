namespace IMSBackend.DTOs.SalesReturns
{
    public class InvoiceReturnableDetailsDto
    {
        public int InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public string? Status { get; set; }
        public List<InvoiceReturnableItemDto> Items { get; set; } = new List<InvoiceReturnableItemDto>();
    }

    public class InvoiceReturnableItemDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductSKU { get; set; }
        public int? VariantId { get; set; }
        public string? VariantName { get; set; }
        public decimal SoldQuantity { get; set; }
        public decimal PreviouslyReturnedQuantity { get; set; }
        public decimal ReturnableQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal DiscountPercent { get; set; }
    }
}
