using System;
using System.Collections.Generic;
using System.Text;
using TiendaInspire.Shared.Events;

namespace TiendaInspire.Shared
{

    public sealed record UserCreatedEvents(string userId, string email) : IRabbitEvent
    {

        public Guid EventId => Guid.NewGuid();
        public DateTime CreateAt => DateTime.UtcNow;

    }
 
}
