namespace StoreService.Database.Entities;

/// <summary>
/// Item line inside an order with captured calculated price for history.
/// </summary>
public class StoreOrderItem
{
    #region Properties
    public long Id { get; set; }
    public long StoreOrderId { get; set; }
    public long StoreItemId { get; set; }
    public int CalculatedPrice { get; set; }
    #endregion

    #region Navigation
    public StoreOrder? Order { get; set; }
    public StoreItem? StoreItem { get; set; }
    public ICollection<StoreOrderItemDiscount> AppliedDiscounts { get; set; } = new List<StoreOrderItemDiscount>();
    #endregion
}

