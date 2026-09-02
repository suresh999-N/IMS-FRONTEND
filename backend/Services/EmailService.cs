using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace IMSBackend.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body,
            byte[]? attachment = null,
            string? attachmentName = null)
        {
            var senderName = _configuration["EmailSettings:SenderName"];
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var portStr = _configuration["EmailSettings:Port"];
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];

            if (string.IsNullOrWhiteSpace(senderEmail))
            {
                throw new InvalidOperationException("EmailSettings:SenderEmail is not configured.");
            }
            if (string.IsNullOrWhiteSpace(smtpServer))
            {
                throw new InvalidOperationException("EmailSettings:SmtpServer is not configured.");
            }
            if (!int.TryParse(portStr, out var port))
            {
                throw new InvalidOperationException("EmailSettings:Port is invalid or not configured.");
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException("EmailSettings:Username is not configured.");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException("EmailSettings:Password is not configured.");
            }

            var email = new MimeMessage();

            email.From.Add(
                new MailboxAddress(
                    senderName,
                    senderEmail));

            email.To.Add(
                MailboxAddress.Parse(toEmail));

            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };

            // =========================
            // ATTACH PDF
            // =========================

            if (attachment != null &&
                attachmentName != null)
            {
                builder.Attachments.Add(
                    attachmentName,
                    attachment);
            }

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                smtpServer,
                port,
                SecureSocketOptions.SslOnConnect);

            await smtp.AuthenticateAsync(
                username,
                password);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}