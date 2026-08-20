using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs.PurchaseReturns
{
    public class UpdatePurchaseReturnDto
    {
        [Required]
        public int SupplierId { get; set; }

        [Required]
        public int GrnId { get; set; }

        [Required]
        public DateTime ReturnDate { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "At least one return item is required.")]
        public List<CreatePurchaseReturnItemDto> Items { get; set; } = new();
    }
}
