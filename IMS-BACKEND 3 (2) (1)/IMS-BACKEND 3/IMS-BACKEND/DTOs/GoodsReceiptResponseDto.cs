namespace IMSBackend.DTOs
{
    public class GoodsReceiptResponseDto
    {
        public int GrnId { get; set; }

        public string? GrnNumber { get; set; }

        public int? PoId { get; set; }

        public string? PoNumber { get; set; }

        public int? SupplierId { get; set; }

        public string? SupplierName { get; set; }

        public int? WarehouseId { get; set; }

        public string? WarehouseName { get; set; }

        public DateTime? ReceiptDate { get; set; }

        public string? Status { get; set; }

        public string? Notes { get; set; }

        public List<GoodsReceiptItemResponseDto> Items { get; set; } = new();
    }
}