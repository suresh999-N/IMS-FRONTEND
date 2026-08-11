namespace IMSBackend.DTOs.PurchaseReturns
{
    public class GrnItemForReturnResponseDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductSku { get; set; }
        public int? VariantId { get; set; }
        public string? VariantName { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal PreviousReturnedQuantity { get; set; }
        public decimal RemainingReturnableQuantity { get; set; }
        public decimal Price { get; set; }
    }
}
