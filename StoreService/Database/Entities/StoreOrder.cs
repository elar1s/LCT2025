namespace StoreService.Database.Entities;

/// <summary>
/// Represents a purchase order created by a user.
/// </summary>
public class StoreOrder
{
    #region Properties
    public long Id { get; set; }
    public long UserId { get; set; }
    public DateTime CostUpdateDate { get; set; } = DateTime.UtcNow; // updated when prices recalculated
    public DateTime? PaidDate { get; set; }  // when payment succeeded
    public bool ItemsRedeemed { get; set; }  // becomes true once items granted to inventory
    #endregion

    #region Navigation
    public ICollection<StoreOrderItem> OrderItems { get; set; } = new List<StoreOrderItem>();
    #endregion
}

