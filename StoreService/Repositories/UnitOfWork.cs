using Microsoft.EntityFrameworkCore.Storage;
using StoreService.Database;
using StoreService.Database.Entities;

namespace StoreService.Repositories;

/// <summary>
/// Coordinates repository access and database transactions.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    #region Fields
    private readonly ApplicationContext _context;
    private IDbContextTransaction? _transaction;
    #endregion

    #region Ctor
    public UnitOfWork(
        ApplicationContext context,
        IGenericRepository<StoreCategory> storeCategories,
        IGenericRepository<StoreItem> storeItems,
        IGenericRepository<StoreDiscount> storeDiscounts,
        IGenericRepository<StoreDiscountItem> storeDiscountItems,
        IGenericRepository<StoreOrder> storeOrders,
        IGenericRepository<StoreOrderItem> storeOrderItems,
        IGenericRepository<StoreOrderItemDiscount> storeOrderItemDiscounts)
    {
        _context = context;
        StoreCategories = storeCategories;
        StoreItems = storeItems;
        StoreDiscounts = storeDiscounts;
        StoreDiscountItems = storeDiscountItems;
        StoreOrders = storeOrders;
        StoreOrderItems = storeOrderItems;
        StoreOrderItemDiscounts = storeOrderItemDiscounts;
    }
    #endregion

    #region Repositories
    public IGenericRepository<StoreCategory> StoreCategories { get; }
    public IGenericRepository<StoreItem> StoreItems { get; }
    public IGenericRepository<StoreDiscount> StoreDiscounts { get; }
    public IGenericRepository<StoreDiscountItem> StoreDiscountItems { get; }
    public IGenericRepository<StoreOrder> StoreOrders { get; }
    public IGenericRepository<StoreOrderItem> StoreOrderItems { get; }
    public IGenericRepository<StoreOrderItemDiscount> StoreOrderItemDiscounts { get; }
    #endregion

    #region Save
    public bool SaveChanges() => _context.SaveChanges() > 0;

    public async Task<bool> SaveChangesAsync(CancellationToken ct = default) => (await _context.SaveChangesAsync(ct)) > 0;
    #endregion

    #region Transactions
    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null) throw new InvalidOperationException("Transaction already started");
        _transaction = await _context.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction == null) throw new InvalidOperationException("No transaction started");
        try
        {
            await _transaction.CommitAsync(ct);
        }
        catch
        {
            await RollbackTransactionAsync(ct);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction == null) return;
        await _transaction.RollbackAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }
    #endregion

    #region Dispose
    public void Dispose() => _transaction?.Dispose();

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
            await _transaction.DisposeAsync();
    }
    #endregion
}
