namespace IMSBackend.DTOs
{
    public class StockLedgerDto
    {
        public int ProductId { get; set; }

        public int? VariantId { get; set; }

        public int WarehouseId { get; set; }

        public decimal OpeningQty { get; set; }

        public decimal ChangeQty { get; set; }

        public decimal ClosingQty { get; set; }

        public string? TransactionType { get; set; }

        public int? TransactionId { get; set; }
    }
}