using TiendaInspire.Orders.DTOs;
using TiendaInspire.Orders.Entities;
using TiendaInspire.Shared.CommonQuerys;

namespace TiendaInspire.Orders.Services
{
    public interface IOrderService
    {
        
        Task<ServiceResult<IEnumerable<OrderListResponse>>> GetUserOrdersAsync(string userId);
        Task<ServiceResult<OrderResponse>> GetByIdAsync(int id, string userId);
        Task<ServiceResult<OrderResponse>> CreateAsync(string userId, CreateOrderRequest request);
        Task<ServiceResult> CancelAsync(int id, string userId);

        
        Task<ServiceResult<IEnumerable<OrderListResponse>>> GetAllAsync(OrderStatusEnum? status, string? userId);
        Task<ServiceResult<OrderResponse>> GetByIdForAdminAsync(int id);
        Task<ServiceResult> UpdateStatusAsync(int id, OrderStatusEnum newStatus);
    }