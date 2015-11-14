using System.Threading.Tasks;

namespace ExpenseManager.Entity.Providers.infrastructure
{
    public interface IAddOrUpdateDeleteEntityProvider<TEntity>
        where TEntity : BaseEntity
    {
        Task<bool> AddOrUpdateAsync(TEntity entity);
        Task<DeletedEntity<TEntity>> DeteleAsync(TEntity entity);
    }
}