using System.Diagnostics.CodeAnalysis;

namespace StoreService.Database.Entities;

/// <summary>
/// Percentage discount that can apply to one or more store items within a time window.
/// </summary>
public class StoreDiscount
{
    #region Properties
    public long Id { get; set; }
    public decimal Percentage { get; set; }               // 0 - 100 (%)
    public string? Description { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime UntilDate { get; set; }
    public bool IsCanceled { get; set; }
    #endregion

    #region Navigation
    public ICollection<StoreDiscountItem> DiscountItems { get; set; } = new List<StoreDiscountItem>();
    public ICollection<StoreOrderItemDiscount> OrderItemDiscounts { get; set; } = new List<StoreOrderItemDiscount>();
    #endregion

    #region Helpers
    /// <summary>
    /// Checks whether discount is active at provided moment (default: now) ignoring cancellation flag (if canceled returns false).
    /// </summary>
    public bool IsActive(DateTime? at = null)
    {
        if (IsCanceled) return false;
        var moment = at ?? DateTime.UtcNow;
        return moment >= FromDate && moment <= UntilDate;
    }
    #endregion
}

