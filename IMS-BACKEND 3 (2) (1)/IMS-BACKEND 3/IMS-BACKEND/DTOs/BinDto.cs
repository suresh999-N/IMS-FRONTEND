namespace IMSBackend.DTOs
{
    public class BinDto
    {
        public int WarehouseId { get; set; }

        public int RackId { get; set; }

        public string? BinCode { get; set; }

        public decimal? Capacity { get; set; }

        public string? Status { get; set; }
    }
}