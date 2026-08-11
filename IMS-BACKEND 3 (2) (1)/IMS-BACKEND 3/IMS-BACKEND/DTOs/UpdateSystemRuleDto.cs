namespace IMSBackend.DTOs.SystemSettings
{
    public class UpdateSystemRuleDto
    {
        public int RuleId { get; set; }
        public string? RuleValue { get; set; }
        public bool IsEnabled { get; set; }
    }
}