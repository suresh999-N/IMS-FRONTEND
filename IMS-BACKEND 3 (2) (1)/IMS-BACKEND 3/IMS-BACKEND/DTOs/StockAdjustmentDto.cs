namespace IMSBackend.DTOs
{
    public class StockAdjustmentDto
    {
        public int WarehouseId { get; set; }

        public string? AdjustmentType { get; set; }

        public string? Reason { get; set; }
    }
}