using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("otps")]
    public class Otp
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiryTime { get; set; }

        public bool IsUsed { get; set; }

        public string Purpose { get; set; } = string.Empty;

        public User? User { get; set; }
    }
}