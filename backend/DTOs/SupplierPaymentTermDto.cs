namespace IMSBackend.DTOs.Suppliers
{
    public class SupplierPaymentTermDto
    {
        public int CreditDays { get; set; }

        public decimal? CreditLimit { get; set; }

        public string? PaymentMethod { get; set; }

        public string? Notes { get; set; }
    }
}