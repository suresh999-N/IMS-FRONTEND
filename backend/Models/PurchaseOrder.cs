using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("purchase_orders")]
    public class PurchaseOrder
    {
        [Key]
        [Column("po_id")]
        public int PoId { get; set; }

        [Column("supplier_id")]
        public int? SupplierId { get; set; }

        [Column("po_number")]
        public string? PoNumber { get; set; }

        [Column("order_date")]
        public DateTime? OrderDate { get; set; }

        [Column("expected_date")]
        public DateTime? ExpectedDate { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("receiving_status")]
        public string? ReceivingStatus { get; set; }

        [Column("total_amount")]
        public decimal? TotalAmount { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("is_cancelled")]
        public bool IsCancelled { get; set; }

        [Column("cancelled_at")]
        public DateTime? CancelledAt { get; set; }

        [Column("cancellation_reason")]
        public string? CancellationReason { get; set; }
    }
}
