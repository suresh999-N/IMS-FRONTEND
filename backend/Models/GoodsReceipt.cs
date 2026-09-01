using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("goods_receipts")]
    public class GoodsReceipt
    {
        [Key]
        [Column("grn_id")]
        public int GrnId { get; set; }

        [Column("grn_number")]
        [StringLength(50)]
        public string GrnNumber { get; set; } = string.Empty;

        [Column("po_id")]
        public int? PoId { get; set; }

        [Column("supplier_id")]
        public int? SupplierId { get; set; }

        [Column("warehouse_id")]
        public int? WarehouseId { get; set; }

        [Column("receipt_date")]
        public DateTime? ReceiptDate { get; set; }

        public string? SupplierInvoice { get; set; }

        public DateTime? SupplierInvoiceDate { get; set; }

        [Column("status")]
        public string? Status { get; set; }

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
