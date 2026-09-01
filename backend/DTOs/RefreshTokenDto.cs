using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}