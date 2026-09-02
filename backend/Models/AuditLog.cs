using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("audit_logs")]
    public class AuditLog
    {
        [Key]
        [Column("log_id")]
        public int LogId { get; set; }

        [Column("user_id")]
        public int? UserId { get; set; }

        [Column("action")]
        public string? Action { get; set; }

        [Column("module")]
        public string? Module { get; set; }

        [Column("table_name")]
        public string? TableName { get; set; }

        [Column("record_id")]
        public int? RecordId { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}