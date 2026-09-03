using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("suppliers")]
public class Supplier
{
    [Key]
    [Column("supplier_id")]
    public int SupplierId { get; set; }

    [Column("supplier_code")]
    public string? SupplierCode { get; set; }

    public string? Name { get; set; }

    [NotMapped]
    public string? CompanyName { get; set; }

    public string? Category { get; set; }

    [Column("gst_number")]
    public string? GstNumber { get; set; }

    [Column("pan_number")]
    public string? PanNumber { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    [Column("status", TypeName = "varchar(20)")]
    [MaxLength(20)]
    public string? Status { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }
}
