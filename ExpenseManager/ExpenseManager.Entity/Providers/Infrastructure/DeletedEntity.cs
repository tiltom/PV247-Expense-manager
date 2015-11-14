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