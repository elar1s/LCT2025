using System.Linq.Expressions;

namespace StoreService.Repositories;

/// <summary>
/// Generic repository abstraction for simple CRUD & query operations.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
public interface IGenericRepository<TEntity> where TEntity : class
{
    #region Query
    IQueryable<TEntity> Get(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] includes);

    TEntity? GetById(object id);
    Task<TEntity?> GetByIdAsync(object id);
    #endregion

    #region Mutations
    void Add(TEntity entity);
    Task AddAsync(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);

    void Update(TEntity entity);

    void Delete(object id);
    void Delete(TEntity entity);
    void DeleteRange(IEnumerable<TEntity> entities);
    #endregion
}

