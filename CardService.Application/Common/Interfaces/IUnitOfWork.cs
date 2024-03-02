namespace CardService.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> Complete(CancellationToken cancellationToken = default);

        void Dispose();

        IRepository<TEntity> Repository<TEntity>() where TEntity : class;
    }
}
