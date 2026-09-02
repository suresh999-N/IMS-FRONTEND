using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("stock_transfer_items")]
    public class StockTransferItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("transfer_id")]
        public int TransferId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("variant_id")]
        public int? VariantId { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }
    }
}