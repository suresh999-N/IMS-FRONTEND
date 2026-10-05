using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace IMSBackend.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService>? _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService>? logger = null)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body,
            byte[]? attachment = null,
            string? attachmentName = null)
        {
            try
            {
                var senderName = _configuration["EmailSettings:SenderName"];
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var portStr = _configuration["EmailSettings:Port"];
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];

                if (string.IsNullOrWhiteSpace(senderEmail))
                {
                    _logger?.LogWarning("EmailSettings:SenderEmail is not configured. Email to {ToEmail} skipped.", toEmail);
                    return;
                }
                if (string.IsNullOrWhiteSpace(smtpServer))
                {
                    _logger?.LogWarning("EmailSettings:SmtpServer is not configured. Email to {ToEmail} skipped.", toEmail);
                    return;
                }
                if (!int.TryParse(portStr, out var port))
                {
                    _logger?.LogWarning("EmailSettings:Port is invalid. Email to {ToEmail} skipped.", toEmail);
                    return;
                }
                if (string.IsNullOrWhiteSpace(username))
                {
                    _logger?.LogWarning("EmailSettings:Username is not configured. Email to {ToEmail} skipped.", toEmail);
                    return;
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    _logger?.LogWarning("EmailSettings:Password is not configured. Email to {ToEmail} skipped.", toEmail);
                    return;
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
            catch (Exception ex)
            {
                _logger?.LogError(
                    ex,
                    "Failed to deliver email to {ToEmail}. Subject: {Subject}. Error: {Message}",
                    toEmail,
                    subject,
                    ex.Message);
            }
        }
    }
}