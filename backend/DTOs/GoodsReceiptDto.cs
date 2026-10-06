using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class GoodsReceiptDto
    {
        [Required(ErrorMessage = "Purchase order is required.")]
        public int PoId { get; set; }

        [Required(ErrorMessage = "Supplier is required.")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Warehouse is required.")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "Receipt date is required.")]
        public DateTime ReceiptDate { get; set; }

        [Required(ErrorMessage = "Supplier invoice number is required.")]
        public string? SupplierInvoice { get; set; }

        [Required(ErrorMessage = "Supplier invoice date is required.")]
        public DateTime? SupplierInvoiceDate { get; set; }

        public string? Notes { get; set; }

        public List<GoodsReceiptItemDto> Items { get; set; } = new();
    }
}