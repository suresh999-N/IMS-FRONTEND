using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("invoices")]
    public class Invoice
    {
        [Key]
        [Column("invoice_id")]
        public int InvoiceId { get; set; }

        [Column("so_id")]
        public int? SoId { get; set; }

        [Column("customer_id")]
        public int? CustomerId { get; set; }

        [Column("invoice_number")]
        public string? InvoiceNumber { get; set; }

        [Column("invoice_date")]
        public DateTime? InvoiceDate { get; set; }

        [Column("due_date")]
        public DateTime? DueDate { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("paid_amount")]
        public decimal PaidAmount { get; set; }

        [Column("balance_amount")]
        public decimal BalanceAmount { get; set; }

        [Column("is_cancelled")]
        public bool IsCancelled { get; set; }

        [Column("cancelled_at")]
        public DateTime? CancelledAt { get; set; }

        [Column("cancellation_reason")]
        public string? CancellationReason { get; set; }

        // =========================
        // NAVIGATION PROPERTIES
        // =========================

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        public ICollection<InvoiceItem>? InvoiceItems { get; set; }
    }
}
