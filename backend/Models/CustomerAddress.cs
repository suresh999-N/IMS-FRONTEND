using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("customer_addresses")]
    public class CustomerAddress
    {
        [Key]
        [Column("address_id")]
        public int AddressId { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("address_type")]
        public string? AddressType { get; set; }

        [Column("address_line")]
        public string? AddressLine { get; set; }

        [Column("address_line2")]
        public string? AddressLine2 { get; set; }

        [Column("city")]
        public string? City { get; set; }

        [Column("state")]
        public string? State { get; set; }

        [Column("country")]
        public string? Country { get; set; }

        [Column("pincode")]
        public string? Pincode { get; set; }

        [Column("is_primary")]
        public bool IsPrimary { get; set; }

    }
}
