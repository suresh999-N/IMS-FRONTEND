using System;
using System.Collections.Generic;

namespace IMSBackend.DTOs.PurchaseReturns
{
    public class PurchaseReturnResponseDto
    {
        public int ReturnId { get; set; }
        public int Id => ReturnId;
        public string ReturnNumber { get; set; } = string.Empty;

        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string SupplierCode { get; set; } = string.Empty;

        public int GrnId { get; set; }
        public string GrnNumber { get; set; } = string.Empty;
        public DateTime? GrnDate { get; set; }

        public DateTime ReturnDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalReturnAmount => TotalAmount;
        public decimal GrandTotal => TotalAmount;
        public string? Reason { get; set; }
        public string? Notes { get; set; }

        public string Status { get; set; } = "Draft";
        public string? SubmittedBy { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectedBy { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? RejectionReason { get; set; }
        public string? CompletedBy { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int ItemCount { get; set; }
        public decimal TotalQuantity { get; set; }

        public List<PurchaseReturnItemResponseDto> Items { get; set; } = new List<PurchaseReturnItemResponseDto>();
    }

    public class PurchaseReturnItemResponseDto
    {
        public int Id { get; set; }
        public int ReturnId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;

        public int? VariantId { get; set; }
        public string? VariantName { get; set; }

        public decimal Quantity { get; set; }
        public decimal ReturnQuantity => Quantity;

        public decimal Price { get; set; }
        public decimal UnitPrice => Price;
        public decimal LineTotal => Quantity * Price;
    }

    public class RejectPurchaseReturnDto
    {
        public string Reason { get; set; } = string.Empty;
    }
}
