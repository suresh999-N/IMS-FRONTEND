namespace IMSBackend.DTOs
{
    public class StockDto
    {
        public int ProductId { get; set; }

        public int? VariantId { get; set; }

        public int WarehouseId { get; set; }

        public decimal Quantity { get; set; }

        public decimal ReservedQuantity { get; set; }
    }
}