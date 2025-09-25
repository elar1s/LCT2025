using Microsoft.AspNetCore.Mvc;
using StoreService.Models;
using StoreService.Services;

namespace StoreService.Controllers;

/// <summary>
/// Endpoints for order lifecycle: create, query, pay and redeem.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    #region Fields
    private readonly IOrderService _orderService;
    #endregion

    #region Ctor
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    #endregion

    #region Endpoints
    /// <summary>
    /// Creates a new order for specified user and store items. Prices are calculated with active discounts.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var order = await _orderService.CreateOrderAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }

    /// <summary>
    /// Retrieves an order by id.
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] long id, CancellationToken ct)
    {
        var order = await _orderService.GetOrderAsync(id, ct);
        return order == null ? NotFound() : Ok(order);
    }

    /// <summary>
    /// Pays (confirms) an order. Sets the paid date. Idempotent except it fails if already paid.
    /// </summary>
    [HttpPost("{id:long}/pay")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Pay([FromRoute] long id, CancellationToken ct)
    {
        var order = await _orderService.PayOrderAsync(id, ct);
        return Ok(order);
    }

    /// <summary>
    /// Marks items as redeemed (granted to user inventory) after payment.
    /// </summary>
    [HttpPost("{id:long}/redeem")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Redeem([FromRoute] long id, CancellationToken ct)
    {
        var order = await _orderService.RedeemOrderItemsAsync(id, ct);
        return Ok(order);
    }
    #endregion
}

