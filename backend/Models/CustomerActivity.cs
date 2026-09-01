using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("customer_activity")]
    public class CustomerActivity
    {
        [Key]
        [Column("activity_id")]
        public int ActivityId { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("activity_type")]
        public string? ActivityType { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}