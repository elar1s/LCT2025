using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using LctMonolith.Infrastructure.Data;

namespace LctMonolith.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation for common CRUD and query composition.
/// </summary>
public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<TEntity> Set;

    public GenericRepository(AppDbContext context)
    {
        Context = context;
        Set = context.Set<TEntity>();
    }

    public IQueryable<TEntity> Query(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = Set;
        if (filter != null) query = query.Where(filter);
        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }
        if (orderBy != null) query = orderBy(query);
        return query;
    }

    public async Task<TEntity?> GetByIdAsync(object id) => await Set.FindAsync(id) ?? null;

    public ValueTask<TEntity?> FindAsync(params object[] keyValues) => Set.FindAsync(keyValues);

    public async Task AddAsync(TEntity entity, CancellationToken ct = default) => await Set.AddAsync(entity, ct);

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default) => await Set.AddRangeAsync(entities, ct);

    public void Update(TEntity entity) => Set.Update(entity);

    public void Remove(TEntity entity) => Set.Remove(entity);

    public async Task RemoveByIdAsync(object id, CancellationToken ct = default)
    {
        var entity = await Set.FindAsync([id], ct);
        if (entity == null) throw new KeyNotFoundException($"Entity {typeof(TEntity).Name} id={id} not found");
        Set.Remove(entity);
    }
}

