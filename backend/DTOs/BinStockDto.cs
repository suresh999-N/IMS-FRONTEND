namespace IMSBackend.DTOs
{
    public class BinStockDto
    {
        public int ProductId { get; set; }

        public int? VariantId { get; set; }

        public int WarehouseId { get; set; }

        public int BinId { get; set; }

        public decimal Quantity { get; set; }
    }
}