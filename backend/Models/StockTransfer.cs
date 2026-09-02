using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("stock_transfers")]
    public class StockTransfer
    {
        [Key]
        [Column("transfer_id")]
        public int TransferId { get; set; }

        [Column("from_warehouse_id")]
        public int FromWarehouseId { get; set; }

        [Column("to_warehouse_id")]
        public int ToWarehouseId { get; set; }

        [Column("transfer_date")]
        public DateTime TransferDate { get; set; }

        [Column("status")]
        public string? Status { get; set; }
    }
}