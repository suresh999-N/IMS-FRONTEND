namespace IMSBackend.DTOs
{
    public class ApplyPermissionTemplateDto
    {
        public int RoleId { get; set; }

        public string Template { get; set; } = string.Empty;
    }
}