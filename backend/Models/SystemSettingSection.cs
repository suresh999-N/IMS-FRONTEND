using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models.SystemSettings
{
    [Table("system_setting_sections")]
    public class SystemSettingSection
    {
        [Key]
        [Column("section_id")]
        public int SectionId { get; set; }

        [Column("section_key")]
        public string SectionKey { get; set; } = string.Empty;

        [Column("section_name")]
        public string SectionName { get; set; } = string.Empty;

        [Column("display_order")]
        public int DisplayOrder { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        public List<SystemSettingRule> Rules { get; set; } = new();
    }
}