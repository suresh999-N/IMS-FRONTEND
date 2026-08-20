namespace IMSBackend.DTOs.Suppliers
{
    public class CreateSupplierDto
    {
        // BASIC INFO
        public string? SupplierCode { get; set; }

        public string? Name { get; set; }

        public string? Category { get; set; }

        public string? GstNumber { get; set; }

        public string? PanNumber { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Website { get; set; }

        public string? Status { get; set; }

        // CHILD COLLECTIONS
        public List<SupplierContactDto> Contacts { get; set; } = new();

        public List<SupplierAddressDto> Addresses { get; set; } = new();

        public SupplierPaymentTermDto? PaymentTerm { get; set; }

        public List<SupplierBankAccountDto> BankAccounts { get; set; } = new();
    }
}
