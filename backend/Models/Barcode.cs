using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("barcodes")]
    public class Barcode
    {
        [Key]
        [Column("barcode_id")]
        public int BarcodeId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("code_value")]
        public string? CodeValue { get; set; }

        [Column("code_type")]
        public string? CodeType { get; set; }

        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}