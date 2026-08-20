using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("supplier_payment_terms")]
public class SupplierPaymentTerm
{
    [Key]
    [Column("term_id")]
    public int TermId { get; set; }

    [Column("supplier_id")]
    public int SupplierId { get; set; }

    [Column("credit_days")]
    public int CreditDays { get; set; }

    [Column("credit_limit")]
    public decimal? CreditLimit { get; set; }

    [Column("payment_method")]
    public string? PaymentMethod { get; set; }

    public string? Notes { get; set; }
}