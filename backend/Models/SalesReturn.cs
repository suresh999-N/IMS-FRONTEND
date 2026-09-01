using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("sales_returns")]
    public class SalesReturn
    {
        [Key]
        [Column("return_id")]
        public int SalesReturnId { get; set; }

        [Column("return_number")]
        public string? ReturnNumber { get; set; }

        [Column("invoice_id")]
        public int InvoiceId { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("warehouse_id")]
        public int? WarehouseId { get; set; }

        [Column("return_date")]
        public DateTime ReturnDate { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("tax_amount")]
        public decimal TaxAmount { get; set; }

        [Column("discount_amount")]
        public decimal DiscountAmount { get; set; }

        [Column("grand_total")]
        public decimal GrandTotal { get; set; }

        [Column("refund_amount")]
        public decimal RefundAmount { get; set; }

        [Column("status")]
        public string Status { get; set; } = "Draft";

        [Column("reason")]
        public string? Reason { get; set; }

        [Column("rejection_reason")]
        public string? RejectionReason { get; set; }

        [Column("approved_by")]
        public string? ApprovedBy { get; set; }

        [Column("approved_at")]
        public DateTime? ApprovedAt { get; set; }

        [Column("refund_method")]
        public string? RefundMethod { get; set; }

        [Column("refund_reference")]
        public string? RefundReference { get; set; }

        [Column("refund_date")]
        public DateTime? RefundDate { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        // Foreign Keys / Navigation Properties
        [ForeignKey("InvoiceId")]
        public Invoice? Invoice { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        [ForeignKey("WarehouseId")]
        public Warehouse? Warehouse { get; set; }

        public ICollection<SalesReturnItem> Items { get; set; } = new List<SalesReturnItem>();
    }
}
