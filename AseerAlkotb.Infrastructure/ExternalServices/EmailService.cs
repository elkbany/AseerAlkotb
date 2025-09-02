using AseerAlkotb.Application.Contracts.External;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Infrastructure.ExternalServices
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _email;
        private readonly string _appPassword;
        private readonly bool _isBodyHtml;
        public EmailService(IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection("EmailSettings");
            _smtpServer = emailSettings["SmtpServer"] ?? "smtp.gmail.com";
            _port = int.TryParse(emailSettings["Port"], out int port) ? port : 587;
            _email = emailSettings["Email"] ?? "default@example.com";
            _appPassword = emailSettings["AppPassword"] ?? "default-password";
            _isBodyHtml = bool.TryParse(emailSettings["IsBodyHtml"], out bool isHtml) && isHtml;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using (var client = new SmtpClient(_smtpServer, _port))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_email, _appPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_email),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = _isBodyHtml
                };

                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
