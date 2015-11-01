using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.Entity.Providers.infrastructure
{
    public class DeletedEntity<TEntity>
        where TEntity : BaseEntity
    {
        public DeletedEntity(TEntity deletedEntity)
        {
            Entity = deletedEntity;
            Succeeded = deletedEntity != null;
        }

        public TEntity Entity { get; private set; }

        public bool Succeeded { get; private set; }
    }
}
