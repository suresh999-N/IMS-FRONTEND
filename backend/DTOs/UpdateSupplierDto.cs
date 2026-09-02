using System.ComponentModel.DataAnnotations;
using IMSBackend.DTOs.Suppliers;

public class UpdateSupplierDto
{
    [MaxLength(40, ErrorMessage = "Supplier code cannot exceed 40 characters.")]
    public string? SupplierCode { get; set; }

    [MaxLength(120, ErrorMessage = "Supplier name cannot exceed 120 characters.")]
    [RegularExpression(@"^[A-Za-z0-9\s.,&'/\-()]+$", ErrorMessage = "Name contains invalid characters.")]
    public string? Name { get; set; }

    [MaxLength(150, ErrorMessage = "Company name cannot exceed 150 characters.")]
    public string? CompanyName { get; set; }

    [MaxLength(100, ErrorMessage = "Category cannot exceed 100 characters.")]
    public string? Category { get; set; }
    [MaxLength(15, ErrorMessage = "GSTIN cannot exceed 15 characters.")]
    public string? GstNumber { get; set; }
    [MaxLength(10, ErrorMessage = "PAN cannot exceed 10 characters.")]
    public string? PanNumber { get; set; }
    [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
    public string? Phone { get; set; }
    [Required(ErrorMessage = "Email is required.")]
    [MaxLength(254, ErrorMessage = "Email address cannot exceed 254 characters.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string? Email { get; set; }
    [MaxLength(150, ErrorMessage = "Website URL cannot exceed 150 characters.")]
    public string? Website { get; set; }
    [MaxLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
    public string? Status { get; set; }

    public List<SupplierContactDto> Contacts { get; set; } = new();

    public List<SupplierAddressDto> Addresses { get; set; } = new();

    public SupplierPaymentTermDto? PaymentTerm { get; set; }

    public List<SupplierBankAccountDto> BankAccounts { get; set; } = new();
}
