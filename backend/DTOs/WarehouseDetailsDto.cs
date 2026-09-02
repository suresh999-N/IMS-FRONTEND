namespace IMSBackend.DTOs
{
    public class WarehouseDetailsDto
    {
        public int WarehouseId { get; set; }

        public string WarehouseName { get; set; } = string.Empty;

        public int TotalRacks { get; set; }

        public int TotalBins { get; set; }

        public int TotalProducts { get; set; }

        public decimal TotalStockUnits { get; set; }

        public List<WarehouseProductDto> Products { get; set; } = new();
    }

    public class WarehouseProductDto
    {
        public string ProductName { get; set; } = string.Empty;

        public string RackCode { get; set; } = string.Empty;

        public string BinCode { get; set; } = string.Empty;

        public decimal Quantity { get; set; }
    }
}