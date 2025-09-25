using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StoreService.Database;

namespace StoreService.Repositories;

/// <summary>
/// Generic repository implementation wrapping EF Core DbSet.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    #region Fields
    private readonly ApplicationContext _context;
    private readonly DbSet<TEntity> _dbSet;
    #endregion

    #region Ctor
    public GenericRepository(ApplicationContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }
    #endregion

    #region Query
    public virtual IQueryable<TEntity> Get(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;

        if (filter != null)
            query = query.Where(filter);

        foreach (var include in includes)
            query = query.Include(include);

        return orderBy != null ? orderBy(query) : query;
    }

    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;
        foreach (var include in includes)
            query = query.Include(include);
        return await query.FirstOrDefaultAsync(predicate);
    }

    public virtual TEntity? GetById(object id) => _dbSet.Find(id);

    public virtual async Task<TEntity?> GetByIdAsync(object id) => await _dbSet.FindAsync(id);
    #endregion

    #region Mutations
    public virtual void Add(TEntity entity) => _dbSet.Add(entity);
    public virtual async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);
    public virtual void AddRange(IEnumerable<TEntity> entities) => _dbSet.AddRange(entities);

    public virtual void Update(TEntity entity) => _dbSet.Update(entity);

    public virtual void Delete(object id)
    {
        var entity = _dbSet.Find(id) ?? throw new KeyNotFoundException($"Entity {typeof(TEntity).Name} with id '{id}' not found");
        Delete(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
            _dbSet.Attach(entity);
        _dbSet.Remove(entity);
    }

    public virtual void DeleteRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);
    #endregion
}

