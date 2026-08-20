using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("sales_returns")]
    public class SalesReturn
    {
        [Key]
        [Column("SalesReturnId")]
        public int SalesReturnId { get; set; }

        [Column("ReturnNumber")]
        [StringLength(50)]
        public string ReturnNumber { get; set; } = string.Empty;

        [Column("CustomerId")]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        [Column("InvoiceId")]
        public int InvoiceId { get; set; }

        [ForeignKey("InvoiceId")]
        public Invoice? Invoice { get; set; }

        [Column("ReturnDate")]
        public DateTime ReturnDate { get; set; }

        [Column("Reason")]
        public string Reason { get; set; } = string.Empty;

        [Column("TotalReturnAmount", TypeName = "decimal(18,2)")]
        public decimal TotalReturnAmount { get; set; } = 0.00m;

        [Column("Status")]
        [StringLength(30)]
        public string Status { get; set; } = "Draft";

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        public ICollection<SalesReturnItem> Items { get; set; } = new List<SalesReturnItem>();
    }
}
