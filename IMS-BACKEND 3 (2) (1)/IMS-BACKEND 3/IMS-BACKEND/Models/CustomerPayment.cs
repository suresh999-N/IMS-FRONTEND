using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("customer_payments")]
    public class CustomerPayment
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Column("customer_id")]
        public int? CustomerId { get; set; }

        [Column("invoice_id")]
        public int? InvoiceId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [NotMapped]
        public string? PaymentNumber { get; set; }

        [Column("payment_date")]
        public DateTime PaymentDate { get; set; }

        [Column("payment_method")]
        public string? PaymentMethod { get; set; }

        [Column("reference_number")]
        public string? ReferenceNumber { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [NotMapped]
        public string? Status { get; set; }

        [NotMapped]
        public decimal OutstandingBefore { get; set; }

        [NotMapped]
        public decimal OutstandingAfter { get; set; }

        [NotMapped]
        public string? CreatedBy { get; set; }

        [NotMapped]
        public DateTime CreatedAt { get; set; }

        [Column("is_cancelled")]
        public bool IsCancelled { get; set; }

        [Column("cancelled_at")]
        public DateTime? CancelledAt { get; set; }

        [Column("cancellation_reason")]
        public string? CancellationReason { get; set; }

    }
}
