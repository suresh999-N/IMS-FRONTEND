namespace IMSBackend.DTOs
{
    public class PurchaseOrderDto
    {
        public int SupplierId { get; set; }

        public int ProductId { get; set; }

        public int? VariantId { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public decimal Tax { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? ExpectedDate { get; set; }

        public string? Notes { get; set; }

        public int? SourceIndentId { get; set; }

        public string? SourceIndentNumber { get; set; }
    }
}
