using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("stock_ledger")]
    public class StockLedger
    {
        [Key]
        [Column("ledger_id")]
        public int LedgerId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Column("warehouse_id")]
        public int WarehouseId { get; set; }

        [Column("opening_qty")]
        public decimal OpeningQty { get; set; }

        [Column("change_qty")]
        public decimal ChangeQty { get; set; }

        [Column("closing_qty")]
        public decimal ClosingQty { get; set; }

        [Column("transaction_type")]
        public string? TransactionType { get; set; }

        [Column("transaction_id")]
        public int? TransactionId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

    }
}
