namespace IMSBackend.DTOs.Suppliers;

public class UploadSupplierDocumentDto
{
    public string DocumentType { get; set; } = string.Empty;

    public IFormFile File { get; set; } = null!;
}
