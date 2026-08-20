namespace IMSBackend.DTOs
{
    public class BinTransferDto
    {
        public int ProductId { get; set; }

        public int? VariantId { get; set; }

        public int FromBinId { get; set; }

        public int ToBinId { get; set; }

        public decimal Quantity { get; set; }
    }
}