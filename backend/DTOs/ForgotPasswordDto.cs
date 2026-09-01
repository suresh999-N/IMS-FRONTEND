using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public sealed class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;
    }
}
