using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(50, ErrorMessage = "Full name cannot exceed 50 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [MaxLength(254, ErrorMessage = "Email address cannot exceed 254 characters.")]
        [RegularExpression(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,24}$",
            ErrorMessage = "Enter a valid email address format and domain extension."
        )]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [MaxLength(10, ErrorMessage = "Mobile number must be exactly 10 digits.")]
        [RegularExpression(
            @"^[6-9]\d{9}$",
            ErrorMessage = "Mobile number must start with 6, 7, 8, or 9 and be exactly 10 digits."
        )]
        public string PhoneNumber { get; set; } = string.Empty;

        public string? EmployeeId { get; set; }

        public string? Department { get; set; }

        public string? Warehouse { get; set; }

        public string? ProfilePhoto { get; set; }
    }
}