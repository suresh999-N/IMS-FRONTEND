using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class GoodsReceiptDto : IValidatableObject
    {
        public int PoId { get; set; }

        public int? PurchaseOrderId { get; set; }

        [Required(ErrorMessage = "Supplier is required.")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Warehouse is required.")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "Receipt date is required.")]
        public DateTime ReceiptDate { get; set; }

        public string? SupplierInvoice { get; set; }

        public string? SupplierInvoiceNo { get; set; }

        public string? SupplierInvoiceNumber { get; set; }

        public string? InvoiceNo { get; set; }

        public string? InvoiceNumber { get; set; }

        public DateTime? SupplierInvoiceDate { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? Notes { get; set; }

        public List<GoodsReceiptItemDto> Items { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var effectivePoId = PoId > 0 ? PoId : (PurchaseOrderId ?? 0);
            if (effectivePoId <= 0)
            {
                yield return new ValidationResult("Purchase order is required.", new[] { nameof(PoId) });
            }

            var invoiceNo = !string.IsNullOrWhiteSpace(SupplierInvoice)
                ? SupplierInvoice.Trim()
                : (!string.IsNullOrWhiteSpace(SupplierInvoiceNo)
                    ? SupplierInvoiceNo.Trim()
                    : (!string.IsNullOrWhiteSpace(SupplierInvoiceNumber)
                        ? SupplierInvoiceNumber.Trim()
                        : (!string.IsNullOrWhiteSpace(InvoiceNo)
                            ? InvoiceNo.Trim()
                            : InvoiceNumber?.Trim())));

            if (string.IsNullOrWhiteSpace(invoiceNo) || string.Equals(invoiceNo, "N/A", StringComparison.OrdinalIgnoreCase))
            {
                yield return new ValidationResult("Supplier invoice number is required.", new[] { nameof(SupplierInvoice) });
            }

            var invoiceDate = SupplierInvoiceDate ?? InvoiceDate;
            if (!invoiceDate.HasValue)
            {
                yield return new ValidationResult("Supplier invoice date is required.", new[] { nameof(SupplierInvoiceDate) });
            }
        }
    }
}