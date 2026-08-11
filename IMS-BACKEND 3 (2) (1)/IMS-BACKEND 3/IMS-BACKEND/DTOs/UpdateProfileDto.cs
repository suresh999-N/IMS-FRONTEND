namespace IMSBackend.DTOs
{
    public class UpdateProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? EmployeeId { get; set; }
        public string? Department { get; set; }
        public string? Warehouse { get; set; }
        public string? ProfilePhoto { get; set; }
    }
}