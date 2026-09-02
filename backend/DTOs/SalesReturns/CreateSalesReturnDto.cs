using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs.SalesReturns
{
    public class CreateSalesReturnDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        public int? WarehouseId { get; set; }

        [Required]
        public DateTime ReturnDate { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public bool SubmitForApproval { get; set; } = false;

        [Required]
        [MinLength(1, ErrorMessage = "At least one return item is required.")]
        public List<CreateSalesReturnItemDto> Items { get; set; } = new();
    }
}