using System.Text.Json.Serialization;

namespace IMSBackend.DTOs
{
    public class GoodsReceiptItemDto
    {
        public int ProductId { get; set; }

        public int? VariantId { get; set; }

        public decimal QuantityReceived { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice
        {
            get => Price;
            set => Price = value;
        }

        public decimal Discount { get; set; }

        public decimal Tax { get; set; }

        public decimal TaxPercentage { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TaxableAmount { get; set; }

       
        public decimal LineTotal { get; set; }
    }
}