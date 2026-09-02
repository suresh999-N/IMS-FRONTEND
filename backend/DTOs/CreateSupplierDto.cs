using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs.Suppliers
{
    public class CreateSupplierDto
    {
        // BASIC INFO
        public string? SupplierCode { get; set; }

        [RegularExpression(@"^[A-Za-z0-9\s.,&'/\-()]+$", ErrorMessage = "Name contains invalid characters.")]
        public string? Name { get; set; }

        public string? CompanyName { get; set; }

        public string? Category { get; set; }

        public string? GstNumber { get; set; }
        public string? PanNumber { get; set; }

        public string? Phone { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string? Email { get; set; }

        [MaxLength(150, ErrorMessage = "Website URL cannot exceed 150 characters.")]
        public string? Website { get; set; }

        public string? Status { get; set; }

        // CHILD COLLECTIONS
        public List<SupplierContactDto> Contacts { get; set; } = new();

        public List<SupplierAddressDto> Addresses { get; set; } = new();

        public SupplierPaymentTermDto? PaymentTerm { get; set; }

        public List<SupplierBankAccountDto> BankAccounts { get; set; } = new();
    }
}
