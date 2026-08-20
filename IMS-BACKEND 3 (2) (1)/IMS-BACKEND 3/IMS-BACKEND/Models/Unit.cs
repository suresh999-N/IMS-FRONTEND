using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("units")]
    public class Unit
    {
        [Key]
        [Column("unit_id")]
        public int UnitId { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("short_name")]
        public string ShortName { get; set; } = string.Empty;

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        public ICollection<PurchaseIndentItem> PurchaseIndentItems { get; set; }
            = new List<PurchaseIndentItem>();
    }
}
