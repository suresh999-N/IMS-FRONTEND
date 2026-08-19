using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class UpdateProfileDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(50)]
        public string? EmployeeId { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }

        [MaxLength(150)]
        public string? Warehouse { get; set; }

        [MaxLength(500)]
        public string? ProfilePhoto { get; set; }
    }
}