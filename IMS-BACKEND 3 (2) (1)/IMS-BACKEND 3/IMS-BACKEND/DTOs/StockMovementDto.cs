namespace IMSBackend.DTOs
{
    public class StockMovementDto
    {
        public int ProductId { get; set; }

        public int? VariantId { get; set; }

        public int WarehouseId { get; set; }

        public string? MovementType { get; set; }

        public decimal Quantity { get; set; }

        public int? ReferenceId { get; set; }

        public string? ReferenceType { get; set; }

        public string? Notes { get; set; }
    }
}