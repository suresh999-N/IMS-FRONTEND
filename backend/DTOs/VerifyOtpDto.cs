using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class VerifyOtpDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Otp { get; set; } = string.Empty;
    }
}