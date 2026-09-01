using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("roles")]
    public class Role
    {
        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("role_name")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name can contain only letters and spaces.")]
        public string RoleName { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        public ICollection<RolePermission>? RolePermissions { get; set; }
    }
}