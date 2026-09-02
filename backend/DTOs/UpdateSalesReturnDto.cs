namespace IMSBackend.DTOs.SalesReturns
{
    public class UpdateSalesReturnDto
    {
        public int? WarehouseId { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public bool SubmitForApproval { get; set; } = false;
        public List<CreateSalesReturnItemDto> Items { get; set; } = new List<CreateSalesReturnItemDto>();
    }

    public class SalesReturnStatusUpdateDto
    {
        public string? Comments { get; set; }
        public string? Reason { get; set; }
    }

    public class ProcessRefundDto
    {
        public string RefundMethod { get; set; } = "Cash"; // Cash, Bank, UPI, Wallet, Credit Note
        public string? RefundReference { get; set; }
        public DateTime? RefundDate { get; set; }
        public decimal? Amount { get; set; }
        public string? Notes { get; set; }
    }
}
