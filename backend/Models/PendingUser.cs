public class PendingUser
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string PasswordHash { get; set; }

    public string Role { get; set; }

    public string EmailVerificationToken { get; set; }

    public DateTime EmailVerificationTokenExpiry { get; set; }
}