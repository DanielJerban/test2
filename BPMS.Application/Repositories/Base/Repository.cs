using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BPMS.Application.Repositories.Base;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly BpmsDbContext Context;

    public Repository(BpmsDbContext context)
    {
        Context = context;
    }

    public TEntity Get(Guid id)
    {
        return Context.Set<TEntity>().Find(id);
    }

    public bool Any(Expression<Func<TEntity, bool>> predicate)
    {
        return Context.Set<TEntity>().Any(predicate);
    }

    public IEnumerable<TEntity> GetAll()
    {
        return Context.Set<TEntity>();
    }

    public IQueryable<TEntity> GetAsQueryable()
    {
        return Context.Set<TEntity>().AsQueryable();
    }

    public IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeExpressions)
    {
        var dbSet = Context.Set<TEntity>();

        IQueryable<TEntity> query = null;
        foreach (var includeExpression in includeExpressions)
        {
            query = dbSet.Include(includeExpression);
        }

        return query ?? dbSet;
    }
    public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
    {
        return Context.Set<TEntity>().Where(predicate);
    }

    public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return Context.Set<TEntity>().SingleOrDefault(predicate);
    }

    public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return Context.Set<TEntity>().FirstOrDefault(predicate);
    }

    public void Add(TEntity entity)
    {
        Context.Set<TEntity>().Add(entity);
    }
    public void AddOrUpdate(TEntity entity)
    {
        Context.Set<TEntity>().Update(entity);
    }
    public void Update(TEntity entity)
    {
        var entry = Context.Entry(entity);
        entry.State = EntityState.Modified;

    }
    public void AddRange(IEnumerable<TEntity> entities)
    {
        Context.Set<TEntity>().AddRange(entities);
    }

    public void Remove(TEntity entity)
    {
        Context.Set<TEntity>().Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        Context.Set<TEntity>().RemoveRange(entities);
    }

    public IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        return Context.Set<TEntity>().Where(predicate);
    }

    public TEntity Single(Expression<Func<TEntity, bool>> predicate)
    {
        return Context.Set<TEntity>().Single(predicate);
    }
}