namespace IMSBackend.DTOs.PurchaseReturns
{
    public class GrnForReturnResponseDto
    {
        public int GrnId { get; set; }
        public string GrnNumber { get; set; } = string.Empty;
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string? Status { get; set; }
    }
}
