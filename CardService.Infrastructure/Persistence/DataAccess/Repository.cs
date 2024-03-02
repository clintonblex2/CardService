using CardService.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CardService.Infrastructure.Persistence.DataAccess
{
    public class Repository<TEntity, TContext> : IRepository<TEntity> where TEntity : class where TContext : DbContext
    {
        internal TContext context;
        internal DbSet<TEntity> dbSet;

        public Repository(TContext context)
        {
            this.context = context;
            dbSet = context.Set<TEntity>();
        }

        public bool Exist(Expression<Func<TEntity, bool>> predicate)
        {
            var exist = dbSet.Where(predicate);
            return exist.Any();
        }

        public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> match)
        {
            return await dbSet.FirstOrDefaultAsync(match);
        }

        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>>? filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = dbSet;

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(filter);
        }

        public void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public IQueryable<TEntity> FilterAsNoTracking(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int? page = null,
            int? pageSize = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
                query = dbSet.AsNoTracking().Where(filter);

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (include != null)
            {
                query = include(query).AsNoTracking();
            }
            if (page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }
            return query.AsNoTracking();
        }
    }
}
