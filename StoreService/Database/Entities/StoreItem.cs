namespace StoreService.Database.Entities;

/// <summary>
/// Store item with pricing and purchase rules.
/// </summary>
public class StoreItem
{
    #region Properties
    public long Id { get; set; }
    public long ItemId { get; set; }          // FK to external Item service (not modeled here)
    public long StoreCategoryId { get; set; }  // FK to StoreCategory
    public long? RankId { get; set; }          // Minimum rank required to buy (external system)
    public int ManaBuyPrice { get; set; }
    public int ManaSellPrice { get; set; }
    public bool UnlimitedPurchase { get; set; }
    public int InventoryLimit { get; set; }
    #endregion

    #region Navigation
    public StoreCategory? Category { get; set; }
    public ICollection<StoreDiscountItem> DiscountItems { get; set; } = new List<StoreDiscountItem>();
    public ICollection<StoreOrderItem> OrderItems { get; set; } = new List<StoreOrderItem>();
    #endregion
}

