using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
