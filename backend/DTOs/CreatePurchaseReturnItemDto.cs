using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs.PurchaseReturns
{
    public class CreatePurchaseReturnItemDto
    {
        [Required]
        public int ProductId { get; set; }

        public int? VariantId { get; set; }

        [Required]
        [Range(0.001, double.MaxValue,
            ErrorMessage = "Return quantity must be greater than zero.")]
        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        // Compatibility property for existing backend logic.
        // Existing controller code using ReturnQuantity can remain unchanged.
        public decimal ReturnQuantity => Quantity;
    }
}