namespace TiendaInspire.Orders.Entities
{
    public class Order
    {

        public int Id { get; set; }
        public required string UserId { get; set; }
        public OrderStatusEnum Status { get; set; } = OrderStatusEnum.Pending;
        public decimal TotalAmount { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<OrderItem> Items { get; set; } = [];
    }
}
