namespace StoreService.Models;

/// <summary>
/// Order line DTO.
/// </summary>
public class OrderItemDto
{
    public long Id { get; set; }
    public long StoreItemId { get; set; }
    public int CalculatedPrice { get; set; }
    public List<long> AppliedDiscountIds { get; set; } = new();
}

