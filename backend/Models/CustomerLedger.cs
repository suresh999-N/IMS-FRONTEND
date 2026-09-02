using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("customer_ledger")]
    public class CustomerLedger
    {
        [Key]
        [Column("ledger_id")]
        public int LedgerId { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("transaction_type")]
        public string? TransactionType { get; set; }

        [Column("transaction_id")]
        public int? TransactionId { get; set; }

        [Column("debit")]
        public decimal Debit { get; set; }

        [Column("credit")]
        public decimal Credit { get; set; }

        [Column("balance")]
        public decimal Balance { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}