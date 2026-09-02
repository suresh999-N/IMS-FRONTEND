namespace IMSBackend.DTOs.SalesReturns
{
    public class SalesReturnInvoiceDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime? InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
