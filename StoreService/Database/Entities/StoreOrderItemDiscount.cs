namespace StoreService.Database.Entities;

/// <summary>
/// Captures which discounts were applied to order items at purchase time.
/// </summary>
public class StoreOrderItemDiscount
{
    #region Properties
    public long Id { get; set; }
    public long StoreOrderItemId { get; set; }
    public long? StoreDiscountId { get; set; } // can be null if discount later removed but kept for history
    #endregion

    #region Navigation
    public StoreOrderItem? OrderItem { get; set; }
    public StoreDiscount? Discount { get; set; }
    #endregion
}

