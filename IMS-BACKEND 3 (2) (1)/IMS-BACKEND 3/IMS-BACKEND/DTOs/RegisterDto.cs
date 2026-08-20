using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [MaxLength(254)]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,24}$", ErrorMessage = "Enter a valid email address and domain extension.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(10)]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Mobile number must start with 6, 7, 8, or 9 and be exactly 10 digits.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(128)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

        [MaxLength(40)]
        public string? Role { get; set; }
    }
}