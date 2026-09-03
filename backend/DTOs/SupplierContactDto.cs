using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs.Suppliers
{
    public class SupplierContactDto
    {
        public int? ContactId { get; set; }

        [RegularExpression(
            @"^[A-Za-z\s]+$",
            ErrorMessage = "Name can contain only letters and spaces."
        )]
        public string? Name { get; set; }

        public string? Designation { get; set; }

        public string? Department { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public bool IsPrimary { get; set; }
    }
}