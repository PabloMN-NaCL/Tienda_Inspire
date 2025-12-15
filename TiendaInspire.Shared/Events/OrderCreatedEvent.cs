using System;
using System.Collections.Generic;
using System.Text;

namespace TiendaInspire.Shared.Events
{


    public sealed record OrderCreatedEvent(
            int OrderId,
            string UserId,
            string UserEmail, 
            IEnumerable<OrderItemEvent> Items) : IRabbitEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime CreateAt { get; init; } = DateTime.UtcNow;
    }

    public sealed record OrderItemEvent(
        int ProductId,
        string ProductName,
        int Quantity);
}