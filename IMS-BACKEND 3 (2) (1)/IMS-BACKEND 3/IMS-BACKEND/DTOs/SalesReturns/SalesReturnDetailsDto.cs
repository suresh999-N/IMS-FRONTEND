namespace IMSBackend.DTOs.SalesReturns
{
    public class SalesReturnDetailsDto
    {
        public int SalesReturnId { get; set; }
        public string ReturnNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public decimal TotalReturnAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<SalesReturnItemDetailsDto> Items { get; set; } = new();
    }
}
