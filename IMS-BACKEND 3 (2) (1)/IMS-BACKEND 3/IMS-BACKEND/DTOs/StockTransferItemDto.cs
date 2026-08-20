namespace IMSBackend.DTOs
{
    public class StockTransferItemDto
    {
        public int TransferId { get; set; }

        public int ProductId { get; set; }

        public int? VariantId { get; set; }

        public decimal Quantity { get; set; }
    }
}