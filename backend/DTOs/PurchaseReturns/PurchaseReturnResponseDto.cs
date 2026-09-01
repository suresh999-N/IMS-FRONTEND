namespace IMSBackend.DTOs.PurchaseReturns
{
    public class PurchaseReturnResponseDto
    {
        public int PurchaseReturnId { get; set; }
        public string ReturnNumber { get; set; } = string.Empty;
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public int? GrnId { get; set; }
        public string? GrnNumber { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public decimal TotalReturnAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<PurchaseReturnItemResponseDto> Items { get; set; } = new();
    }
}
