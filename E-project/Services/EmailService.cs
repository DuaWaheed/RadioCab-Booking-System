using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace E_project.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("RadioCab", emailSettings["FromEmail"]));
            emailMessage.To.Add(MailboxAddress.Parse(toEmail));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message
            };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            // ✅ Safely parse port or use fallback (587)
            int smtpPort = int.TryParse(emailSettings["SmtpPort"], out var port) ? port : 587;

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(emailSettings["SmtpHost"], smtpPort, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(emailSettings["SmtpUsername"], emailSettings["SmtpPassword"]);
                await smtpClient.SendAsync(emailMessage);
                await smtpClient.DisconnectAsync(true);
            }
        }
    }
}
