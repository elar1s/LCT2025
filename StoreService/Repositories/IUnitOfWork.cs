using Microsoft.EntityFrameworkCore.Storage;
using StoreService.Database.Entities;

namespace StoreService.Repositories;

/// <summary>
/// Unit of work pattern abstraction encapsulating repositories and transactions.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable, IDisposable
{
    #region Repositories
    IGenericRepository<StoreCategory> StoreCategories { get; }
    IGenericRepository<StoreItem> StoreItems { get; }
    IGenericRepository<StoreDiscount> StoreDiscounts { get; }
    IGenericRepository<StoreDiscountItem> StoreDiscountItems { get; }
    IGenericRepository<StoreOrder> StoreOrders { get; }
    IGenericRepository<StoreOrderItem> StoreOrderItems { get; }
    IGenericRepository<StoreOrderItemDiscount> StoreOrderItemDiscounts { get; }
    #endregion

    #region Save
    bool SaveChanges();
    Task<bool> SaveChangesAsync(CancellationToken ct = default);
    #endregion

    #region Transactions
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
    #endregion
}

