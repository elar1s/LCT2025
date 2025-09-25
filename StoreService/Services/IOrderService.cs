using StoreService.Models;

namespace StoreService.Services;

/// <summary>
/// Service responsible for order lifecycle (create, pay, redeem) including price calculation.
/// </summary>
public interface IOrderService
{
    #region Methods
    Task<OrderDto> CreateOrderAsync(CreateOrderRequest request, CancellationToken ct = default);
    Task<OrderDto?> GetOrderAsync(long id, CancellationToken ct = default);
    Task<OrderDto> PayOrderAsync(long id, CancellationToken ct = default);
    Task<OrderDto> RedeemOrderItemsAsync(long id, CancellationToken ct = default);
    #endregion
}

