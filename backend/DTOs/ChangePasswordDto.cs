using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class ChangePasswordDto : IValidatableObject
    {
        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [MaxLength(128, ErrorMessage = "Password cannot exceed 128 characters.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\d\s]).{8,}$",
            ErrorMessage = "Password must contain at least 1 uppercase letter, 1 lowercase letter, 1 number, and 1 special character."
        )]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(CurrentPassword) &&
                !string.IsNullOrWhiteSpace(NewPassword) &&
                string.Equals(CurrentPassword.Trim(), NewPassword.Trim(), StringComparison.Ordinal))
            {
                yield return new ValidationResult(
                    "New password cannot be the same as the current password.",
                    new[] { nameof(NewPassword) });
            }

            if (!string.IsNullOrWhiteSpace(NewPassword) &&
                !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                !string.Equals(NewPassword, ConfirmPassword, StringComparison.Ordinal))
            {
                yield return new ValidationResult(
                    "New password and confirm password do not match.",
                    new[] { nameof(ConfirmPassword) });
            }
        }
    }
}