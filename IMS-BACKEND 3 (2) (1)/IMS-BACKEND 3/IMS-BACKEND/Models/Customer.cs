using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("customers")]
    public class Customer
    {
        [Key]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("customer_code")]
        public string? CustomerCode { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("company")]
        public string? Company { get; set; }

        [Column("gst_number")]
        public string? GstNumber { get; set; }

        [Column("pan_number")]
        public string? PanNumber { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("city")]
        public string? City { get; set; }

        [Column("credit_limit")]
        public decimal CreditLimit { get; set; }

        [Column("outstanding_balance")]
        public decimal OutstandingBalance { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}