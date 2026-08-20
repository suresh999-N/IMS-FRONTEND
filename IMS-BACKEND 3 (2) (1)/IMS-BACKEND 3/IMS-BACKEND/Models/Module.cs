using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("modules")]
    public class Module
    {
        [Key]
        public int ModuleId { get; set; }

        public string ModuleKey { get; set; } = string.Empty;

        public string ModuleName { get; set; } = string.Empty;

        public string? Category { get; set; }

        public string? Description { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; }
            = new List<RolePermission>();
    }
}