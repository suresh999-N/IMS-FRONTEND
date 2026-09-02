using System.ComponentModel.DataAnnotations;

namespace IMSBackend.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(254)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;

        [MaxLength(10)]
        public string? PhoneNumber { get; set; }

        public string? EmployeeId { get; set; }

        public string? Department { get; set; }

        public string? Warehouse { get; set; }

        public string? ProfilePhoto { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int TokenVersion { get; set; } = 1;

        public int FailedLoginAttempts { get; set; } = 0;

        public DateTime? LockoutEnd { get; set; }

        public bool IsEmailVerified { get; set; } = false;

        public string? EmailVerificationToken { get; set; }

        public DateTime? EmailVerificationTokenExpiry { get; set; }
    }
}