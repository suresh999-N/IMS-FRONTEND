namespace IMSBackend.DTOs
{
    public class GoodsReceiptDto
    {
        public int PoId { get; set; }

        public int SupplierId { get; set; }

        public int WarehouseId { get; set; }

        public DateTime ReceiptDate { get; set; }

        public string? SupplierInvoice { get; set; }

        public DateTime? SupplierInvoiceDate { get; set; }

        public string? Notes { get; set; }

        public List<GoodsReceiptItemDto> Items { get; set; } = new();
    }
}