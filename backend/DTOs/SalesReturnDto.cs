namespace IMSBackend.DTOs.SalesReturns
{
    public class SalesReturnDto
    {
        public int SalesReturnId { get; set; }
        public string? ReturnNumber { get; set; }
        public int InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal RefundAmount { get; set; }
        public string Status { get; set; } = "Draft";
        public string? Reason { get; set; }
        public string? RejectionReason { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RefundMethod { get; set; }
        public string? RefundReference { get; set; }
        public DateTime? RefundDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<SalesReturnItemDto> Items { get; set; } = new List<SalesReturnItemDto>();
    }
}
