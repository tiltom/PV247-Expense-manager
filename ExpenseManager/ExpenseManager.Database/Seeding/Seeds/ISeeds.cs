using System.Data.Entity;

namespace ExpenseManager.Database.Seeding.Seeds
{
    internal interface ISeeds<TContext>
        where TContext : DbContext
    {
        void Seed(TContext context);
    }
}