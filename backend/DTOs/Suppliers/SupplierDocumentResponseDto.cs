namespace IMSBackend.DTOs.Suppliers;

public class SupplierDocumentResponseDto
{
    public int DocumentId { get; set; }

    public int SupplierId { get; set; }

    public string DocumentType { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string OriginalFileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSizeInBytes { get; set; }

    public string Status { get; set; } = string.Empty;

    public bool IsTemporary { get; set; }

    public DateTime UploadedAt { get; set; }
}
