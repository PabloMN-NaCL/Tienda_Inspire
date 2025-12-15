using MassTransit;
using Microsoft.Extensions.Logging;
using TiendaInspire.Shared.Events;
using TiendInspire.Notificaciones.Email;
using System.Threading.Tasks;

namespace TiendInspire.Notificaciones
{
   
    public class OrderCanceledConsumer : IConsumer<OrderCancelledEvent>
    {
        private readonly ILogger<OrderCanceledConsumer> _logger;
        private readonly IEmailService _emailService;

     
        public OrderCanceledConsumer(ILogger<OrderCanceledConsumer> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

   
        public Task Consume(ConsumeContext<OrderCancelledEvent> context)
        {
           
            var cancelEvent = context.Message;

         
            string userEmail = cancelEvent.UserEmail;

            _logger.LogInformation("Pedido Cancelado Recibido. ID: {OrderId}, Usuario: {UserId}, Email para enviar: {UserEmail}",
                cancelEvent.OrderId, cancelEvent.UserId, userEmail);

            _emailService.SendOrderCancellationMail(userEmail, cancelEvent.OrderId);

           
            return Task.CompletedTask;
        }
    }
}