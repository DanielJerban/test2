using System.Linq.Expressions;


namespace BPMS.Infrastructure.Repositories.Base;

public interface IRepository<TEntity> where TEntity : class
{
    TEntity Get(Guid id);
    IEnumerable<TEntity> GetAll();
    IQueryable<TEntity> GetAsQueryable();
    IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeExpressions);
    IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
    TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
    TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
    TEntity Single(Expression<Func<TEntity, bool>> predicate);
    void Add(TEntity entity);
    void AddOrUpdate(TEntity entity);
    void Update(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
    bool Any(Expression<Func<TEntity, bool>> predicate);
}