using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("racks")]
    public class Rack
    {
        [Key]
        [Column("rack_id")]
        public int RackId { get; set; }

        [Column("warehouse_id")]
        public int WarehouseId { get; set; }

        [Column("zone_id")]
        public int? ZoneId { get; set; }

        [Column("rack_code")]
        public string? RackCode { get; set; }

        [Column("description")]
        public string? Description { get; set; }
    }
}