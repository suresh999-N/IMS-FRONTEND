using System.ComponentModel.DataAnnotations;

public class PendingUser
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(254)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(10)]
    public string PhoneNumber { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string EmailVerificationToken { get; set; } = string.Empty;

    public DateTime EmailVerificationTokenExpiry { get; set; }
}