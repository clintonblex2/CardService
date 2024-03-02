using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CardService.Application.Common.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> match);

        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>>? filter = null, params Expression<Func<TEntity, object>>[] includes);

        bool Exist(Expression<Func<TEntity, bool>> predicate);

        void Insert(TEntity entity);

        IQueryable<TEntity> FilterAsNoTracking(
            Expression<Func<TEntity,
            bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int? page = null,
            int? pageSize = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
    }
}
