using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [MinLength(2, ErrorMessage = "Full Name must be at least 2 characters.")]
        [MaxLength(50, ErrorMessage = "Full Name cannot exceed 50 characters.")]
        [RegularExpression(@"^(?=.*[a-zA-Z\p{L}])[a-zA-Z\p{L}\s'-]+$", ErrorMessage = "Name must contain only valid letters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a valid email address.")]
        [MaxLength(254, ErrorMessage = "Please enter a valid email address.")]
        [RegularExpression(@"^(?!.*\.\.)[a-zA-Z0-9_%+-]+(?:\.[a-zA-Z0-9_%+-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z]{2,24}$", ErrorMessage = "Please enter a valid email address.")]
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