using System;
using System.Collections.Generic;
using System.Text;

namespace TiendaInspire.Shared.Events
{
    public sealed record OrderCancelledEvent(
      int OrderId,
      string UserId,
      IEnumerable<OrderItemEvent> Items) : IRabbitEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime CreateAt { get; init; } = DateTime.UtcNow;
    }
}
