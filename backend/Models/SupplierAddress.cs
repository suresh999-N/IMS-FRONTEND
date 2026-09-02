using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("supplier_addresses")]
public class SupplierAddress
{
    [Key]
    [Column("address_id")]
    public int AddressId { get; set; }

    [Column("supplier_id")]
    public int SupplierId { get; set; }

    [Column("address_type")]
    public string? AddressType { get; set; }

    [Column("address_line")]
    public string? AddressLine { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    [Column("pincode")]
    public string? Pincode { get; set; }
}