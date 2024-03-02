using CardService.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace CardService.Infrastructure.Persistence.DataAccess
{
    public class UnitofWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        private readonly TContext _context;
        private Hashtable _repositories;

        public UnitofWork(TContext context)
        {
            _context = context;
        }

        public async Task<bool> Complete(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            _repositories ??= new Hashtable();
            var type = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repositoryTypeDefinition = typeof(Repository<,>).GetGenericTypeDefinition();
                var repositoryType = repositoryTypeDefinition.MakeGenericType(typeof(TEntity), typeof(TContext));
                var repositoryInstance = Activator.CreateInstance(repositoryType, _context);

                _repositories.Add(type, repositoryInstance);
            }
            return (IRepository<TEntity>)_repositories[type];
        }
    }
}
