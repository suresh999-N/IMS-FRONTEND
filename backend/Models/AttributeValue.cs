using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("attribute_values")]
    public class AttributeValue
    {
        [Key]
        [Column("value_id")]
        public int ValueId { get; set; }

        [Column("attribute_id")]
        public int AttributeId { get; set; }

        [Column("value")]
        public string Value { get; set; } = string.Empty;
    }
}