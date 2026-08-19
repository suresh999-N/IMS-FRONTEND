using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(10)]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Mobile number must start with 6, 7, 8, or 9 and be exactly 10 digits.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User";

        public bool IsActive { get; set; }
    }
}