using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class ResendLoginOtpDto
    {
        public string? Email { get; set; }
        public int? UserId { get; set; }
    }
}