namespace IMSBackend.DTOs
{
    public class StockAdjustmentItemDto
    {
        public int AdjustmentId { get; set; }

        public int ProductId { get; set; }

        public int? VariantId { get; set; }

        public decimal Quantity { get; set; }
    }
}