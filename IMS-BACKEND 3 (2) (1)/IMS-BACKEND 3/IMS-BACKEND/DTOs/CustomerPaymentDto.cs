namespace IMSBackend.DTOs
{
    public class CustomerPaymentDto
    {
        public int CustomerId { get; set; }

        public int? InvoiceId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string? PaymentMethod { get; set; }

        public string? ReferenceNumber { get; set; }

        public string? Notes { get; set; }

        public string? Status { get; set; }

        public string? CreatedBy { get; set; }
    }

    public class UpdateCustomerPaymentDto
    {
        public decimal Amount { get; set; }

        public string? PaymentMethod { get; set; }

        public string? ReferenceNumber { get; set; }

        public string? Notes { get; set; }

        public string? Status { get; set; }
    }
}
