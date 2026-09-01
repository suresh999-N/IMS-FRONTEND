using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("customer_contacts")]
    public class CustomerContact
    {
        [Key]
        [Column("contact_id")]
        public int ContactId { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("name")]
        public string? ContactName { get; set; }

        [Column("designation")]
        public string? Designation { get; set; }

        [Column("role")]
        public string? Role { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("is_primary")]
        public bool IsPrimary { get; set; }

    }
}
