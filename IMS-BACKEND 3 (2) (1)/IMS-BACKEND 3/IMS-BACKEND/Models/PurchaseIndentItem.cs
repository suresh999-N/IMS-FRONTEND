using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("purchase_indent_items")]
    public class PurchaseIndentItem
    {
        [Key]
        [Column("purchase_indent_item_id")]
        public int PurchaseIndentItemId { get; set; }

        [Column("purchase_indent_id")]
        public int PurchaseIndentId { get; set; }

        [ForeignKey(nameof(PurchaseIndentId))]
        public PurchaseIndent PurchaseIndent { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [Column("unit_id")]
        public int UnitId { get; set; }

        [ForeignKey(nameof(UnitId))]
        public Unit Unit { get; set; }

        [Column("required_qty")]
        public decimal RequiredQty { get; set; }

        [Column("available_stock")]
        public decimal AvailableStock { get; set; }

        [Column("required_date")]
        public DateTime RequiredDate { get; set; }

        [Column("remarks")]
        public string? Remarks { get; set; }
    }
}