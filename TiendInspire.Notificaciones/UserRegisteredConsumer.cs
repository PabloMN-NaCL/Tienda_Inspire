using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using TiendaInspire.Shared;
using TiendInspire.Notificaciones.Email;

namespace TiendInspire.Notificaciones
{
    internal class UserRegisteredConsumer:IConsumer
    {
        private ILogger<UserRegisteredConsumer> _logger;
        private IEmailService _emailService;
        public UserRegisteredConsumer(ILogger<UserRegisteredConsumer> logger,
            IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;

        }
        public Task Consume(ConsumeContext<UserCreatedEvents> context)
        {
            var user = context.Message;
            _logger.LogInformation("New user registered: {UserId}, Email: {Email}", 
                user.userId, user.email);
            _emailService.SendWelcomeEmail(user.email);

            return Task.CompletedTask;
        }

    }
}
