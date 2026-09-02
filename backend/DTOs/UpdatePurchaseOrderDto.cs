namespace IMSBackend.DTOs
{
    public class UpdatePurchaseOrderItemDto
    {
        public int? Id { get; set; }

        public int ProductId { get; set; }

        public int? VariantId { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public decimal Tax { get; set; }
    }

    public class UpdatePurchaseOrderDto
    {
        public int SupplierId { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? ExpectedDate { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        public string? Notes { get; set; }

        // Support single item fallback for backward compatibility
        public int? ProductId { get; set; }

        public int? VariantId { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? Price { get; set; }

        public decimal? Discount { get; set; }

        public decimal? Tax { get; set; }

        // Support multi-item update
        public List<UpdatePurchaseOrderItemDto>? Items { get; set; }

        public DateTime? GetEffectiveExpectedDate() => ExpectedDate ?? ExpectedDeliveryDate;
    }
}
