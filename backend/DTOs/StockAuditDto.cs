namespace IMSBackend.DTOs
{
    public class StockAuditDto
    {
        public int WarehouseId { get; set; }

        public DateTime AuditDate { get; set; }

        public string? AuditType { get; set; }

        public string? Status { get; set; }

        public int? CreatedBy { get; set; }

        public int? ApprovedBy { get; set; }

        public string? Notes { get; set; }
    }
}