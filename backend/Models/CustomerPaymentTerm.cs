using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("customer_payment_terms")]
    public class CustomerPaymentTerm
    {
        [Key]
        [Column("term_id")]
        public int PaymentTermId { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("credit_days")]
        public int CreditDays { get; set; }

        [Column("credit_limit")]
        public decimal CreditLimit { get; set; }

        [Column("payment_method")]
        public string? PaymentMode { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

    }
}
