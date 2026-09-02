using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("refresh_tokens")]
    public class RefreshToken
    {
        [Key]
        [Column("RefreshTokenId")]
        public int RefreshTokenId { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        [Required]
        [Column("Token")]
        [MaxLength(500)]
        public string Token { get; set; } = string.Empty;

        [Required]
        [Column("ExpiresAt")]
        public DateTime ExpiresAt { get; set; }

        [Required]
        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("RevokedAt")]
        public DateTime? RevokedAt { get; set; }

        [Column("CreatedByIp")]
        [MaxLength(100)]
        public string? CreatedByIp { get; set; }

        [Column("DeviceName")]
        [MaxLength(200)]
        public string? DeviceName { get; set; }

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        [NotMapped]
        public bool IsActive => RevokedAt == null && !IsExpired;
    }
}