using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("bins")]
    public class Bin
    {
        [Key]
        [Column("bin_id")]
        public int BinId { get; set; }

        [Column("warehouse_id")]
        public int WarehouseId { get; set; }

        [Column("rack_id")]
        public int RackId { get; set; }

        [Column("bin_code")]
        public string? BinCode { get; set; }

        [Column("capacity")]
        public decimal? Capacity { get; set; }

        [Column("status")]
        public string? Status { get; set; }
    }
}