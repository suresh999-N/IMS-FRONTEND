namespace IMSBackend.DTOs
{
    public class BinStockDetailsDto
    {
        public int BinStockId { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;

        public int? VariantId { get; set; }

        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;

        public int RackId { get; set; }
        public string RackCode { get; set; } = string.Empty;

        public int BinId { get; set; }
        public string BinCode { get; set; } = string.Empty;

        public decimal Quantity { get; set; }
    }
}
