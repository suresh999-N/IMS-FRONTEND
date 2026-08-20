using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs.PurchaseIndents
{
    public class CreatePurchaseIndentItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public decimal RequiredQty { get; set; }

        public int UnitId { get; set; }

        public decimal AvailableStock { get; set; }

        public DateTime RequiredDate { get; set; }

        public string? Remarks { get; set; }
    }
}