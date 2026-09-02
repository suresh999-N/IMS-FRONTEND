using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models.SystemSettings
{
    [Table("system_setting_rules")]
    public class SystemSettingRule
    {
        [Key]
        [Column("rule_id")]
        public int RuleId { get; set; }

        [Column("section_id")]
        public int SectionId { get; set; }

        [Column("rule_key")]
        public string RuleKey { get; set; } = string.Empty;

        [Column("rule_name")]
        public string RuleName { get; set; } = string.Empty;

        [Column("rule_description")]
        public string? RuleDescription { get; set; }

        [Column("rule_type")]
        public string RuleType { get; set; } = string.Empty;

        [Column("rule_value")]
        public string? RuleValue { get; set; }

        [Column("default_value")]
        public string? DefaultValue { get; set; }

        [Column("is_enabled")]
        public bool IsEnabled { get; set; }

        [Column("display_order")]
        public int DisplayOrder { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public SystemSettingSection? Section { get; set; }
    }
}