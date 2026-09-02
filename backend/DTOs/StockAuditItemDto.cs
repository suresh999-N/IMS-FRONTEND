namespace IMSBackend.DTOs
{
    public class StockAuditItemDto
    {
        public int? AuditId { get; set; }

        public int? ProductId { get; set; }

        public int? VariantId { get; set; }

        public int? BinId { get; set; }

        public decimal? SystemQuantity { get; set; }

        public decimal? PhysicalQuantity { get; set; }
    }
}