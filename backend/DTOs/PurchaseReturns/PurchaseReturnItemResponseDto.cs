namespace IMSBackend.DTOs.PurchaseReturns
{
    public class PurchaseReturnItemResponseDto
    {
        public int PurchaseReturnItemId { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductSku { get; set; }
        public int? VariantId { get; set; }
        public string? VariantName { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal ReturnQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
