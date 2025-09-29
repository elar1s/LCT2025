using System.Linq.Expressions;

namespace LctMonolith.Infrastructure.Repositories;

/// <summary>
/// Generic repository abstraction for aggregate root / entity access. Read operations return IQueryable for composition.
/// </summary>
public interface IGenericRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Query(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> GetByIdAsync(object id);
    ValueTask<TEntity?> FindAsync(params object[] keyValues);

    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task RemoveByIdAsync(object id, CancellationToken ct = default);
}

