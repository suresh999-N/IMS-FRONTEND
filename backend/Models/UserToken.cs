using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("user_tokens")]
    public class UserToken
    {
        [Key]
        public int TokenId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}