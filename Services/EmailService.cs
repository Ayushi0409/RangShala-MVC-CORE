// Services/EmailService.cs
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace RangShala.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            // Debug: Log settings to verify they’re loaded
            Console.WriteLine($"SmtpServer: {emailSettings["SmtpServer"]}");
            Console.WriteLine($"SmtpPort: {emailSettings["SmtpPort"]}");
            Console.WriteLine($"SenderEmail: {emailSettings["SenderEmail"]}");
            Console.WriteLine($"Username: {emailSettings["Username"]}");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    Console.WriteLine("Connecting to SMTP server...");
                    await client.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
                    Console.WriteLine("Connected successfully.");

                    Console.WriteLine("Authenticating...");
                    await client.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
                    Console.WriteLine("Authenticated successfully.");

                    Console.WriteLine("Sending email...");
                    await client.SendAsync(message);
                    Console.WriteLine("Email sent successfully.");
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    Console.WriteLine("Disconnected from SMTP server.");
                }
            }
        }
    }
}