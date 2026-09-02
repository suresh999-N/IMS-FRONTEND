using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models;

[Table("supplier_documents")]
public class SupplierDocument
{
    [Key]
    [Column("document_id")]
    public int DocumentId { get; set; }

    [Required]
    [Column("supplier_id")]
    public int SupplierId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("document_type")]
    public string DocumentType { get; set; } = string.Empty;

    // ERP display label
    [Required]
    [MaxLength(150)]
    [Column("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    // Actual uploaded filename
    [Required]
    [MaxLength(255)]
    [Column("original_file_name")]
    public string OriginalFileName { get; set; } = string.Empty;

    // Physically stored secure filename
    [Required]
    [MaxLength(255)]
    [Column("stored_file_name")]
    public string StoredFileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    [Column("file_path")]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("content_type")]
    public string ContentType { get; set; } = string.Empty;

    [Column("file_size_bytes")]
    public long FileSizeInBytes { get; set; }

    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = "uploaded";

    [Column("is_temporary")]
    public bool IsTemporary { get; set; } = false;

    [Column("uploaded_at")]
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [ForeignKey(nameof(SupplierId))]
    public Supplier Supplier { get; set; } = null!;
}
