namespace IMSBackend.DTOs.SalesReturns
{
    public class SalesReturnListDto
    {
        public int SalesReturnId { get; set; }
        public string ReturnNumber { get; set; } = string.Empty;
        public int InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalReturnAmount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
