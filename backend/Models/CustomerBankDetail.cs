using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("customer_bank_details")]
    public class CustomerBankDetail
    {
        [Key]
        [Column("bank_id")]
        public int BankDetailId { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("account_name")]
        public string? AccountName { get; set; }

        [Column("account_number")]
        public string? AccountNumber { get; set; }

        [Column("bank_name")]
        public string? BankName { get; set; }

        [Column("ifsc_code")]
        public string? IfscCode { get; set; }

        [Column("branch")]
        public string? Branch { get; set; }

        [Column("is_primary")]
        public bool IsPrimary { get; set; }

    }
}
