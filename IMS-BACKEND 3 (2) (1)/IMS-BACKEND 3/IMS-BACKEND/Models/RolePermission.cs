using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("role_permissions")]
    public class RolePermission
    {
        [Key]
        public int PermissionId { get; set; }

        public int RoleId { get; set; }

        public int ModuleId { get; set; }

        public bool CanView { get; set; }

        public bool CanAdd { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; } = null!;

        [ForeignKey(nameof(ModuleId))]
        public Module Module { get; set; } = null!;
    }
}