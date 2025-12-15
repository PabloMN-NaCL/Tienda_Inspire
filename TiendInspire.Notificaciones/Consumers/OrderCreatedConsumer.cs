using MassTransit;
using Microsoft.Extensions.Logging;
using TiendaInspire.Shared.Events;
using TiendInspire.Notificaciones.Email;
using System.Threading.Tasks;

namespace TiendInspire.Notificaciones
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedConsumer> _logger; 
        private readonly IEmailService _emailService;           

        public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var orderEvent = context.Message;

          
            string userEmail = orderEvent.UserEmail;

            _logger.LogInformation("Procesando Confirmación de Pedido: {OrderId}, Usuario: {UserId}, Email: {Email}",
                orderEvent.OrderId, orderEvent.UserId, userEmail);

            _emailService.SendOrderConfirmationMail(userEmail, orderEvent.OrderId, orderEvent.Items);

            return Task.CompletedTask;
        }
    }
}