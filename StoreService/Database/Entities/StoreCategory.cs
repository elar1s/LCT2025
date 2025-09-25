namespace StoreService.Database.Entities;

/// <summary>
/// Category grouping store items.
/// </summary>
public class StoreCategory
{
    #region Properties
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    #endregion

    #region Navigation
    public ICollection<StoreItem> Items { get; set; } = new List<StoreItem>();
    #endregion
}

