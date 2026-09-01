using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^\\d{6}$")]
        public string Otp { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [MaxLength(128, ErrorMessage = "Password cannot exceed 128 characters.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\d\s]).{8,}$",
            ErrorMessage = "Password must contain at least 1 uppercase letter, 1 lowercase letter, 1 number, and 1 special character."
        )]
        public string NewPassword { get; set; } = string.Empty;
    }
}
