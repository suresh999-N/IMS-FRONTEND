using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("variant_attribute_values")]
    public class VariantAttributeValue
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("variant_id")]
        public int VariantId { get; set; }

        [Column("attribute_id")]
        public int AttributeId { get; set; }

        [Column("value_id")]
        public int ValueId { get; set; }
    }
}