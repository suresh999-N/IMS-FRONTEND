public class StockTransferDto
{
    public int ProductId { get; set; }

    public int? VariantId { get; set; }

    public decimal Quantity { get; set; }

    public int FromWarehouseId { get; set; }

    public int ToWarehouseId { get; set; }

    public DateTime TransferDate { get; set; }

    public string? Status { get; set; }
}