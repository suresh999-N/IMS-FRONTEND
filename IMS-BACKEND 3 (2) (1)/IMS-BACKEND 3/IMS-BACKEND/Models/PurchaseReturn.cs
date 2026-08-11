using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("purchase_returns")]
    public class PurchaseReturn
    {
        [Key]
        [Column("return_id")]
        public int PurchaseReturnId { get; set; }

        [Column("return_number")]
        [StringLength(255)]
        public string ReturnNumber { get; set; } = string.Empty;

        [Column("supplier_id")]
        public int? SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }

        [Column("grn_id")]
        public int? GrnId { get; set; }

        [ForeignKey("GrnId")]
        public GoodsReceipt? GoodsReceipt { get; set; }

        [Column("return_date")]
        public DateTime ReturnDate { get; set; }

        [Column("return_reason")]
        public string Reason { get; set; } = string.Empty;

        [Column("total_return_amount", TypeName = "decimal(18,2)")]
        public decimal TotalReturnAmount { get; set; } = 0.00m;

        [Column("status")]
        [StringLength(30)]
        public string Status { get; set; } = "Completed";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public ICollection<PurchaseReturnItem> Items { get; set; } = new List<PurchaseReturnItem>();
    }
}
