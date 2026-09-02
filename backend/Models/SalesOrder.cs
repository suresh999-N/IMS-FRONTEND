using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("sales_orders")]
    public class SalesOrder
    {
        [Key]
        [Column("so_id")]
        public int SoId { get; set; }

        [Column("customer_id")]
        public int? CustomerId { get; set; }

        [Column("so_number")]
        public string? SoNumber { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}