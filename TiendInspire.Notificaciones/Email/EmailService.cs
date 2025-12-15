using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Text;

namespace TiendInspire.Notificaciones.Email
{
    public class EmailService:IEmailService
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
                Text = "Gracias por hacerte una cuenta en Tienda Inspire  !"
            };

            await client.SendAsync(message);
            _logger.LogInformation("Email de bienvenida enviado a {ToEmail}", toEmail);


        }

        public async Task SendOrderConfirmationMail(string toEmail, int orderId,
            IEnumerable<TiendaInspire.Shared.Events.OrderItemEvent> items)
        {
            using var client = new SmtpClient();
            var host = _configuration.GetSection("Smtp:Host").Value;
            var port = int.Parse(_configuration.GetSection("Smtp:Port").Value!);
            client.Connect(host, port, false);

            var fromEmail = _configuration.GetSection("Email:FromAddress").Value;
            var message = new MimeKit.MimeMessage();

            message.From.Add(new MimeKit.MailboxAddress("Tienda Inspire", fromEmail!));
            message.To.Add(new MimeKit.MailboxAddress("", toEmail));
            message.Subject = $"Confirmación de Pedido #{orderId}";

            var bodyText = new StringBuilder();
            bodyText.AppendLine($"¡Hola! Tu pedido #{orderId} ha sido creado exitosamente.");
            bodyText.AppendLine("Detalles del pedido:");
            foreach (var item in items)
            {
                bodyText.AppendLine($"- {item.ProductName} (ID: {item.ProductId}) - Cantidad: {item.Quantity}");
            }
            bodyText.AppendLine("\n¡Gracias por tu compra!");

            message.Body = new MimeKit.TextPart("plain")
            {
                Text = bodyText.ToString()
            };

            await client.SendAsync(message);
            _logger.LogInformation("Email de confirmación de pedido {OrderId} enviado a {ToEmail}", orderId, toEmail);
        }

        public async Task SendOrderCancellationMail(string toEmail, int orderId)
        {
            using var client = new SmtpClient();
            var host = _configuration.GetSection("Smtp:Host").Value;
            var port = int.Parse(_configuration.GetSection("Smtp:Port").Value!);
            client.Connect(host, port, false);

            var fromEmail = _configuration.GetSection("Email:FromAddress").Value;
            var message = new MimeKit.MimeMessage();

            message.From.Add(new MimeKit.MailboxAddress("Tienda Inspire", fromEmail!));
            message.To.Add(new MimeKit.MailboxAddress("", toEmail));
            message.Subject = $"Cancelación de Pedido #{orderId}";

            message.Body = new MimeKit.TextPart("plain")
            {
                Text = $"Tu pedido #{orderId} ha sido cancelado. Si tienes alguna pregunta, por favor contáctanos."
            };

            await client.SendAsync(message);
            _logger.LogInformation("Email de cancelación de pedido {OrderId} enviado a {ToEmail}", orderId, toEmail);
        }


    }
}

