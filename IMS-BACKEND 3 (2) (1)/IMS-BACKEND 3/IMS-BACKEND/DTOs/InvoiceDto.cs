namespace IMSBackend.DTOs
{
    public class InvoiceDto
    {
        public int? SoId { get; set; }

        public int? CustomerId { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public DateTime? DueDate { get; set; }

        public decimal PaidAmount { get; set; }

        public string? PaymentMethod { get; set; }

        public string? ReferenceNumber { get; set; }

        public List<InvoiceItemDto> Items { get; set; }
            = new();
    }
}
