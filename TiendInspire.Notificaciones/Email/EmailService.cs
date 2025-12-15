using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Text;

namespace TiendInspire.Notificaciones.Email
{
    internal class EmailService:IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;


        }

        public async Task SendWelcomeMail(string toEmail)
        {

            using var client = new SmtpClient();

            var host = _configuration.GetSection("Smtp:Host").Value;
            var port = int.Parse(_configuration.GetSection("Smtp:Port").Value!);
                client.Connect(host, port, false);

            var fromEmail = _configuration.GetSection("Email:FromAddress").Value;
            var message = new MimeKit.MimeMessage();

            message.From.Add(new MimeKit.MailboxAddress("Tienda Inspire", fromEmail!));
            message.To.Add(new MimeKit.MailboxAddress("", toEmail));
            message.Subject = "Bienvenido Tienda Inspire!";
            message.Body = new MimeKit.TextPart("plain")
            {            
                Text = "Gracias por hacerte una cuenta en Tienda Inspire!"
            };

            await client.SendAsync(message);
            _logger.LogInformation("Email de bienvenida enviado a {ToEmail}", toEmail);


        }


    }
}

