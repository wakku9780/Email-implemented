

//namespace WebApiCore.Services
using Microsoft.AspNetCore.SignalR;
using MimeKit;
using WebApiCore.Hubs;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace WebApiCore.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<NotificationHub> _hubContext;

        public EmailService(IConfiguration configuration, IHubContext<NotificationHub> hubContext)
        {
            _configuration = configuration;
            _hubContext = hubContext;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var emailMessage = new MimeMessage
            {
                From = { new MailboxAddress("Your Name", _configuration["EmailSettings:FromEmail"]) },
                To = { new MailboxAddress("", toEmail) },
                Subject = subject,
                Body = new BodyBuilder { HtmlBody = body }.ToMessageBody()
            };

            try
            {
                using (var client = new SmtpClient())
                {
                    // Connect to the SMTP server with TLS (or SSL if needed)
                    await client.ConnectAsync(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:Port"]), SecureSocketOptions.StartTls);

                    // Ignore certificate validation errors (for testing only)
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                    // Authenticate with the SMTP server
                    await client.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);

                    // Send the email
                    await client.SendAsync(emailMessage);

                    // Disconnect from the SMTP server
                    await client.DisconnectAsync(true);
                }

                // Notify clients using SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"Email sent to {toEmail}");
            }
            catch (Exception ex)
            {
                // Log detailed information about exceptions
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }
    }
}
