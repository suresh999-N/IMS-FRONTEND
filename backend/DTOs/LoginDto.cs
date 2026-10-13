using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class LoginDto
    {
        [MaxLength(256)]
        public string EmailOrPhone { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? Email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(128)]
        public string Password { get; set; } = string.Empty;
    }
}