using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class VerifyOtpDto
    {
        public int? UserId { get; set; }

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [MaxLength(254, ErrorMessage = "Email address cannot exceed 254 characters.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "OTP code is required.")]
        public string Otp { get; set; } = string.Empty;
    }
}