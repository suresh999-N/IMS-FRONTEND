using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs.SalesReturns
{
    public class CreateSalesReturnDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        [Required]
        public DateTime ReturnDate { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "At least one return item is required.")]
        public List<CreateSalesReturnItemDto> Items { get; set; } = new();
    }
}
