namespace IMSBackend.DTOs
{
    public class SupplierPaymentDto
    {
        public int SupplierId { get; set; }

        public int PoId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string? PaymentMethod { get; set; }

        public string? ReferenceNumber { get; set; }

        public string? Notes { get; set; }
    }
}