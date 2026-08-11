using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("attributes")]
    public class ProductAttribute
    {
        [Key]
        [Column("attribute_id")]
        public int AttributeId { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;
    }
}