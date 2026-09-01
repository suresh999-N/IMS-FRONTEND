using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("purchase_indents")]
    public class PurchaseIndent
    {
        [Key]
        [Column("purchase_indent_id")]
        public int PurchaseIndentId { get; set; }

        [Column("indent_number")]
        public string? IndentNumber { get; set; }

        [Column("indent_date")]
        public DateTime IndentDate { get; set; }

        [Column("required_date")]
        public DateTime RequiredDate { get; set; }

        [Column("requested_by")]
        public int RequestedBy { get; set; }

        [Column("department_id")]
        public int DepartmentId { get; set; }

        [Column("supplier_id")]
        public int? SupplierId { get; set; }

        [Column("approved_by")]
        public int? ApprovedBy { get; set; }

        [Column("priority")]
        public string? Priority { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("remarks")]
        public string? Remarks { get; set; }

        [Column("total_items")]
        public int TotalItems { get; set; }

        [Column("total_quantity")]
        public decimal TotalQuantity { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(RequestedBy))]
        public User RequestedByUser { get; set; }

        public ICollection<PurchaseIndentItem> Items { get; set; }
            = new List<PurchaseIndentItem>();
    }
}