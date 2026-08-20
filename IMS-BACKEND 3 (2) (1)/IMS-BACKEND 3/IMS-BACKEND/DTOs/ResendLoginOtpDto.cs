using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class ResendLoginOtpDto
    {
        [Required]
        public int UserId { get; set; }
    }
}