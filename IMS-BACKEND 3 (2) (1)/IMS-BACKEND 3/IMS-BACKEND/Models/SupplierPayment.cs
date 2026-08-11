using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("supplier_payments")]
    public class SupplierPayment
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Column("supplier_id")]
        public int? SupplierId { get; set; }

        [Column("po_id")]
        public int? PoId { get; set; }

        [Column("amount")]
        public decimal? Amount { get; set; }

        [Column("payment_date")]
        public DateTime? PaymentDate { get; set; }

        [Column("payment_method")]
        public string? PaymentMethod { get; set; }

        [Column("reference_number")]
        public string? ReferenceNumber { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("is_cancelled")]
        public bool IsCancelled { get; set; }

        [Column("cancelled_at")]
        public DateTime? CancelledAt { get; set; }

        [Column("cancellation_reason")]
        public string? CancellationReason { get; set; }

    }
}
