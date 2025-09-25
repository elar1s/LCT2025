namespace StoreService.Database.Entities;

/// <summary>
/// Join entity linking discounts to store items.
/// </summary>
public class StoreDiscountItem
{
    #region Properties
    public long Id { get; set; }
    public long StoreDiscountId { get; set; }
    public long StoreItemId { get; set; }
    #endregion

    #region Navigation
    public StoreDiscount? Discount { get; set; }
    public StoreItem? StoreItem { get; set; }
    #endregion
}

