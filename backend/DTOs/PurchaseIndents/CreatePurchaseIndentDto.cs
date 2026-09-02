using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs.PurchaseIndents
{
    public class CreatePurchaseIndentDto
    {
        [Required]
        public DateTime IndentDate { get; set; }

        [Required]
        public DateTime RequiredDate { get; set; }

        [Required]
        public int RequestedBy { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public int? SupplierId { get; set; }

        public int? ApprovedBy { get; set; }

        public string? Priority { get; set; }

        public string? Remarks { get; set; }

        public List<CreatePurchaseIndentItemDto> Items { get; set; } = new();
    }
}