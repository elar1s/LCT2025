using Microsoft.EntityFrameworkCore;
using StoreService.Database.Entities;
using StoreService.Models;
using StoreService.Repositories;

namespace StoreService.Services;

/// <summary>
/// Implements order creation, payment, redemption and price calculation logic.
/// </summary>
public class OrderService : IOrderService
{
    #region Fields
    private readonly IUnitOfWork _uow;
    private readonly ILogger<OrderService> _logger;
    #endregion

    #region Ctor
    public OrderService(IUnitOfWork uow, ILogger<OrderService> logger)
    {
        _uow = uow;
        _logger = logger;
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public async Task<OrderDto> CreateOrderAsync(CreateOrderRequest request, CancellationToken ct = default)
    {
        if (request.StoreItemIds.Count == 0)
            throw new ArgumentException("No store items specified", nameof(request.StoreItemIds));

        // Ensure uniqueness
        var uniqueIds = request.StoreItemIds.Distinct().ToList();

        // Load items with potential discount items and discounts for calculation.
        var items = await _uow.StoreItems
            .Get(i => uniqueIds.Contains(i.Id), includes: i => i.DiscountItems)
            .Include(i => i.DiscountItems) // ensure collection loaded
            .ToListAsync(ct);

        if (items.Count != uniqueIds.Count)
        {
            var missing = uniqueIds.Except(items.Select(i => i.Id)).ToArray();
            throw new KeyNotFoundException($"Store items not found: {string.Join(", ", missing)}");
        }

        // Preload discounts referenced by items for efficient calculation.
        var discountIds = items.SelectMany(i => i.DiscountItems.Select(di => di.StoreDiscountId)).Distinct().ToList();
        var discounts = await _uow.StoreDiscounts
            .Get(d => discountIds.Contains(d.Id))
            .ToListAsync(ct);

        var now = DateTime.UtcNow;

        var order = new StoreOrder
        {
            UserId = request.UserId,
            CostUpdateDate = now,
            OrderItems = new List<StoreOrderItem>()
        };

        foreach (var item in items)
        {
            var applicableDiscounts = discounts
                .Where(d => item.DiscountItems.Any(di => di.StoreDiscountId == d.Id) && d.IsActive(now))
                .ToList();

            var calculatedPrice = CalculatePrice(item.ManaBuyPrice, applicableDiscounts);

            var orderItem = new StoreOrderItem
            {
                StoreItemId = item.Id,
                CalculatedPrice = calculatedPrice,
                AppliedDiscounts = applicableDiscounts.Select(d => new StoreOrderItemDiscount
                {
                    StoreDiscountId = d.Id
                }).ToList()
            };

            order.OrderItems.Add(orderItem);
        }

        await _uow.StoreOrders.AddAsync(order);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Created order {OrderId} for user {UserId}", order.Id, order.UserId);

        return MapOrder(order);
    }

    /// <inheritdoc/>
    public async Task<OrderDto?> GetOrderAsync(long id, CancellationToken ct = default)
    {
        var order = await _uow.StoreOrders
            .Get(o => o.Id == id, includes: o => o.OrderItems)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.AppliedDiscounts)
            .FirstOrDefaultAsync(ct);
        return order == null ? null : MapOrder(order);
    }

    /// <inheritdoc/>
    public async Task<OrderDto> PayOrderAsync(long id, CancellationToken ct = default)
    {
        var order = await _uow.StoreOrders
            .Get(o => o.Id == id, includes: o => o.OrderItems)
            .FirstOrDefaultAsync(ct) ?? throw new KeyNotFoundException($"Order {id} not found");

        if (order.PaidDate != null)
            throw new InvalidOperationException("Order already paid");

        order.PaidDate = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
        _logger.Information("Order {OrderId} paid", id);
        return await GetOrderRequiredAsync(id, ct);
    }

    /// <inheritdoc/>
    public async Task<OrderDto> RedeemOrderItemsAsync(long id, CancellationToken ct = default)
    {
        var order = await _uow.StoreOrders.Get(o => o.Id == id).FirstOrDefaultAsync(ct) ??
                    throw new KeyNotFoundException($"Order {id} not found");

        if (order.PaidDate == null)
            throw new InvalidOperationException("Order not paid yet");
        if (order.ItemsRedeemed)
            throw new InvalidOperationException("Order items already redeemed");

        // TODO: integrate with external inventory service. For now we just toggle flag.
        order.ItemsRedeemed = true;
        await _uow.SaveChangesAsync(ct);
        _logger.Information("Order {OrderId} items redeemed", id);
        return await GetOrderRequiredAsync(id, ct);
    }
    #endregion

    #region Helpers
    private int CalculatePrice(int basePrice, IEnumerable<StoreDiscount> discounts)
    {
        // Assumption: sum discount percentages, cap at 90% to avoid free unless explicitly 100%. Documented decision.
        var totalPercent = discounts.Sum(d => d.Percentage);
        if (totalPercent > 100m) totalPercent = 100m; // absolute cap
        var discounted = basePrice - (int)Math.Floor(basePrice * (decimal)totalPercent / 100m);
        return Math.Max(0, discounted);
    }

    private async Task<OrderDto> GetOrderRequiredAsync(long id, CancellationToken ct)
    {
        var dto = await GetOrderAsync(id, ct);
        if (dto == null) throw new KeyNotFoundException($"Order {id} not found after update");
        return dto;
    }

    private static OrderDto MapOrder(StoreOrder order) => new()
    {
        Id = order.Id,
        UserId = order.UserId,
        CostUpdateDate = order.CostUpdateDate,
        PaidDate = order.PaidDate,
        ItemsRedeemed = order.ItemsRedeemed,
        Items = order.OrderItems.Select(oi => new OrderItemDto
        {
            Id = oi.Id,
            StoreItemId = oi.StoreItemId,
            CalculatedPrice = oi.CalculatedPrice,
            AppliedDiscountIds = oi.AppliedDiscounts.Select(ad => ad.StoreDiscountId ?? 0).ToList()
        }).ToList()
    };
    #endregion
}

