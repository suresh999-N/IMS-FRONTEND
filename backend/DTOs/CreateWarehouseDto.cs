using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class CreateWarehouseDto
    {
        [Required]
        [StringLength(150, MinimumLength = 3)]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name can contain only letters and spaces.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^WH-[A-Z]{3,5}-\d{3}$",
            ErrorMessage = "Warehouse code format must be WH-XXX-001")]
        public string WarehouseCode { get; set; } = string.Empty;

        [Required]
        [StringLength(255, MinimumLength = 2)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [StringLength(150, MinimumLength = 3)]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name can contain only letters and spaces.")]
        public string ManagerName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[0-9]{10}$",
            ErrorMessage = "Phone must contain exactly 10 digits")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(active|inactive)$",
            ErrorMessage = "Status must be active or inactive")]
        public string Status { get; set; } = "active";
    }
}