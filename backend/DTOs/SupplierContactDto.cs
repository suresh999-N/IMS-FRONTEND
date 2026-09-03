using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs.Suppliers
{
    public class SupplierContactDto
    {
        public int? ContactId { get; set; }

        public int? Id { get; set; }

        [MaxLength(100, ErrorMessage = "Contact name cannot exceed 100 characters.")]
        [RegularExpression(
            @"^[A-Za-z0-9\s.,&'/\-()]+$",
            ErrorMessage = "Name can contain letters, numbers, spaces, and common punctuation only."
        )]
        public string? Name { get; set; }

        public string? Designation { get; set; }

        public string? Department { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public bool IsPrimary { get; set; }
    }
}