namespace IMSBackend.DTOs
{
    public class VerifyEmailOtpDto
    {
        public string Email { get; set; } = string.Empty;

        public string Otp { get; set; } = string.Empty;
    }
}