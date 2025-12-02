using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using TiendaInspire.Shared;

namespace TiendInspire.Notificaciones
{
    internal class UserRegisteredConsumer:IConsumer
    {
        private ILogger<UserRegisteredConsumer> _logger;
        public UserRegisteredConsumer(ILogger<UserRegisteredConsumer> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<UserCreatedEvents> context)
        {
            var user = context.Message;
            _logger.LogInformation("New user registered: {UserId}, Email: {Email}", 
                user.userId, user.email);

            return Task.CompletedTask;
        }

    }
}
