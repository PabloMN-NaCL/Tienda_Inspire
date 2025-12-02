using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using MassTransit;

namespace TiendaInspire.Shared.Events
{
    [ExcludeFromTopology]
    internal interface IRabbitEvent
    {
        public Guid EventId { get; }
        public DateTime CreateAt {get;}
    }
}
