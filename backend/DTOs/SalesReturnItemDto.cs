namespace IMSBackend.DTOs.SalesReturns
{
    public class SalesReturnItemDto
    {
        public int Id { get; set; }
        public int SalesReturnId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductSKU { get; set; }
        public int? VariantId { get; set; }
        public string? VariantName { get; set; }
        public decimal InvoicedQuantity { get; set; }
        public decimal ReturnQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }

    public class CreateSalesReturnItemDto
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public decimal InvoicedQuantity { get; set; }
        public decimal ReturnQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
    }
}
