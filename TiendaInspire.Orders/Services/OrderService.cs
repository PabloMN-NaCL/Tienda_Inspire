using MassTransit;
using TiendaInspire.Orders.Entities;
using TiendaInspire.Orders.DTOs;
using TiendaInspire.Orders.Data;
using TiendaInspire.Shared.CommonQuerys;
using TiendaInspire.Shared.Events;
using Microsoft.EntityFrameworkCore;

namespace TiendaInspire.Orders.Services
{
    public class OrderService(
      OrdersDbContext dbContext,
      IHttpClientFactory httpClientFactory,
      IPublishEndpoint publishEndpoint,
      ILogger<OrderService> logger) : IOrderService
    {
        public async Task<ServiceResult<IEnumerable<OrderListResponse>>> GetUserOrdersAsync(string userId)
        {
            var orders = await dbContext.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderListResponse(
                    o.Id, o.Status.ToString(), o.TotalAmount, o.Items.Count, o.CreatedAt))
                .ToListAsync();

            return ServiceResult<IEnumerable<OrderListResponse>>.Success(orders);
        }

        public async Task<ServiceResult<OrderResponse>> GetByIdAsync(int id, string userId)
        {
            var order = await dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null)
            {
                return ServiceResult<OrderResponse>.Failure("Order not found");
            }

            if (order.UserId != userId)
            {
                return ServiceResult<OrderResponse>.Failure("Access denied");
            }

            return ServiceResult<OrderResponse>.Success(MapToResponse(order));
        }

        public async Task<ServiceResult<OrderResponse>> CreateAsync(string userId, string? userEmail, CreateOrderRequest request)
        {
            if (!request.Items.Any())
            {
                return ServiceResult<OrderResponse>.Failure("Order must have at least one item");
            }

            var catalogClient = httpClientFactory.CreateClient("catalog");
            var orderItems = new List<OrderItem>();
            var reservedItems = new List<(int ProductId, int Quantity)>();

            foreach (var item in request.Items)
            {
                try
                {
                    // Get product info
                    var response = await catalogClient.GetAsync($"/api/v1/products/{item.ProductId}");
          

                    var product = await response.Content.ReadFromJsonAsync<ProductInfo>();
                    if (product is null)
                    {
                        await ReleaseReservedStockAsync(catalogClient, reservedItems);
                        return ServiceResult<OrderResponse>.Failure($"Could not fetch product {item.ProductId}");
                    }



                    reservedItems.Add((item.ProductId, item.Quantity));

                    orderItems.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        ProductName = product.Name,
                        UnitPrice = product.Price,
                        Quantity = item.Quantity
                    });
                }
                catch (HttpRequestException ex)
                {
                    logger.LogError(ex, "Catalog service unavailable");
                    await ReleaseReservedStockAsync(catalogClient, reservedItems);
                    return ServiceResult<OrderResponse>.Failure("Catalog service unavailable");
                }
            }

            var order = new Order
            {
                UserId = userId,
                ShippingAddress = request.ShippingAddress,
                Notes = request.Notes,
                Items = orderItems,
                TotalAmount = orderItems.Sum(i => i.UnitPrice * i.Quantity)
            };

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Order created: {OrderId} for user {UserId}", order.Id, userId);

           
            var orderCreatedEvent = new OrderCreatedEvent(
                order.Id,
                userId,
                userEmail,
                orderItems.Select(i => new OrderItemEvent(i.ProductId, i.ProductName, i.Quantity)));

            await publishEndpoint.Publish(orderCreatedEvent);

            return ServiceResult<OrderResponse>.Success(MapToResponse(order), "Order created successfully");
        }

        private async Task ReleaseReservedStockAsync(HttpClient catalogClient, List<(int ProductId, int Quantity)> reservedItems)
        {
            foreach (var (productId, quantity) in reservedItems)
            {
                try
                {
                    await catalogClient.PostAsJsonAsync(
                        $"/api/v1/products/{productId}/release",
                        new { Quantity = quantity });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to release reserved stock for product {ProductId}", productId);
                }
            }
        }

        public async Task<ServiceResult> CancelAsync(int id, string userEmail, string userId)
        {
            var order = await dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null)
            {
                return ServiceResult.Failure("Order not found");
            }

            if (order.UserId != userId)
            {
                return ServiceResult.Failure("Access denied");
            }

            if (order.Status is not (OrderStatusEnum.Pending or OrderStatusEnum.Confirmed))
            {
                return ServiceResult.Failure("Order cannot be cancelled at this stage");
            }

            // Release stock via HTTP
            var catalogClient = httpClientFactory.CreateClient("catalog");
            foreach (var item in order.Items)
            {
                try
                {
                    var response = await catalogClient.PostAsJsonAsync(
                        $"/api/v1/products/{item.ProductId}/release",
                        new { Quantity = item.Quantity });

                    if (!response.IsSuccessStatusCode)
                    {
                        logger.LogWarning(
                            "Failed to release stock for product {ProductId} on order {OrderId} cancellation",
                            item.ProductId, id);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex,
                        "Error releasing stock for product {ProductId} on order {OrderId} cancellation",
                        item.ProductId, id);
                }
            }

            order.Status = OrderStatusEnum.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            logger.LogInformation("Order cancelled: {OrderId}", id);

            // Publish event for audit/notifications (stock already released via HTTP)
            var orderCancelledEvent = new OrderCancelledEvent(
                order.Id,
                userId,
                userEmail,
                order.Items.Select(i => new OrderItemEvent(i.ProductId, i.ProductName, i.Quantity)));

            await publishEndpoint.Publish(orderCancelledEvent);

            return ServiceResult.Success("Order cancelled successfully");
        }

        public async Task<ServiceResult<IEnumerable<OrderListResponse>>> GetAllAsync(OrderStatusEnum? status, string? userId)
        {
            var query = dbContext.Orders.AsQueryable();

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(o => o.UserId == userId);

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderListResponse(
                    o.Id, o.Status.ToString(), o.TotalAmount, o.Items.Count, o.CreatedAt))
                .ToListAsync();

            return ServiceResult<IEnumerable<OrderListResponse>>.Success(orders);
        }

        public async Task<ServiceResult<OrderResponse>> GetByIdForAdminAsync(int id)
        {
            var order = await dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null)
            {
                return ServiceResult<OrderResponse>.Failure("Order not found");
            }

            return ServiceResult<OrderResponse>.Success(MapToResponse(order));
        }

        public async Task<ServiceResult> UpdateStatusAsync(int id, OrderStatusEnum newStatus)
        {
            var order = await dbContext.Orders.FindAsync(id);
            if (order is null)
            {
                return ServiceResult.Failure("Order not found");
            }

            if (!IsValidStatusTransition(order.Status, newStatus))
            {
                return ServiceResult.Failure($"Cannot transition from {order.Status} to {newStatus}");
            }

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            logger.LogInformation("Order status updated: {OrderId} -> {Status}", id, newStatus);

            return ServiceResult.Success("Order status updated successfully");
        }

        private static bool IsValidStatusTransition(OrderStatusEnum current, OrderStatusEnum next)
        {
            return (current, next) switch
            {
                (OrderStatusEnum.Pending, OrderStatusEnum.Confirmed) => true,
                (OrderStatusEnum.Pending, OrderStatusEnum.Cancelled) => true,
                (OrderStatusEnum.Confirmed, OrderStatusEnum.Processing) => true,
                (OrderStatusEnum.Confirmed, OrderStatusEnum.Cancelled) => true,
                (OrderStatusEnum.Processing, OrderStatusEnum.Shipped) => true,
                (OrderStatusEnum.Shipped, OrderStatusEnum.Delivered) => true,
                _ => false
            };
        }

        private static OrderResponse MapToResponse(Order order) => new(
            order.Id,
            order.UserId,
            order.Status.ToString(),
            order.TotalAmount,
            order.ShippingAddress,
            order.Notes,
            order.CreatedAt,
            order.UpdatedAt,
            order.Items.Select(i => new OrderItemResponse(
                i.Id, i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.Subtotal)));

        private record ProductInfo(int Id, string Name, decimal Price, int Stock, bool IsActive);
    }
}