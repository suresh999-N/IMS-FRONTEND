using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("purchase_returns")]
    public class PurchaseReturn
    {
        [Key]
        [Column("purchase_return_id")]
        public int PurchaseReturnId { get; set; }

        [Column("return_number")]
        [StringLength(50)]
        public string ReturnNumber { get; set; } = string.Empty;

        [Column("supplier_id")]
        public int SupplierId { get; set; }

        [Column("grn_id")]
        public int GrnId { get; set; }

        [Column("return_date")]
        public DateTime ReturnDate { get; set; } = DateTime.Now;

        [Column("reason")]
        public string Reason { get; set; } = string.Empty;

        [Column("total_return_amount", TypeName = "decimal(18,2)")]
        public decimal TotalReturnAmount { get; set; }

        [Column("status")]
        [StringLength(30)]
        public string Status { get; set; } = "Draft";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public virtual Supplier? Supplier { get; set; }

        [ForeignKey(nameof(GrnId))]
        public virtual GoodsReceipt? GoodsReceipt { get; set; }

        public virtual ICollection<PurchaseReturnItem> Items { get; set; }
            = new List<PurchaseReturnItem>();
    }
}