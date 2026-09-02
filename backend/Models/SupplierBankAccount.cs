using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("supplier_bank_details")]
public class SupplierBankAccount
{
    [Key]
    [Column("bank_id")]
    public int BankId { get; set; }

    [Column("supplier_id")]
    public int SupplierId { get; set; }

    [Column("account_name")]
    public string? AccountName { get; set; }

    [Column("account_number")]
    public string? AccountNumber { get; set; }

    [Column("bank_name")]
    public string? BankName { get; set; }

    [Column("ifsc_code")]
    public string? IfscCode { get; set; }

    public string? Branch { get; set; }

    [Column("bank_state")]
    public string? BankState { get; set; }

    [Column("bank_city")]
    public string? BankCity { get; set; }
}
