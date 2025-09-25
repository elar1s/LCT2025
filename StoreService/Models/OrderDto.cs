namespace StoreService.Models;

/// <summary>
/// Result DTO representing an order summary.
/// </summary>
public class OrderDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public DateTime CostUpdateDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public bool ItemsRedeemed { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

