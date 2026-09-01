using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("supplier_contacts")]
public class SupplierContact
{
    [Key]
    [Column("contact_id")]
    public int ContactId { get; set; }

    [Column("supplier_id")]
    public int SupplierId { get; set; }

    public string? Name { get; set; }

    public string? Designation { get; set; }

    public string? Department { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    [Column("is_primary")]
    public bool IsPrimary { get; set; }
}