namespace IMSBackend.DTOs
{
    public class StockAuditResponseDto
    {
        public int AuditId { get; set; }

        public int WarehouseId { get; set; }

        public DateTime AuditDate { get; set; }

        public string? AuditType { get; set; }

        public string? Status { get; set; }

        public string? CreatedBy { get; set; }

        public string? ApprovedBy { get; set; }

        public string? Notes { get; set; }
    }
}