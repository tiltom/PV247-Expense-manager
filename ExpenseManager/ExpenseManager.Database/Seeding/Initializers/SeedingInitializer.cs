using ExpenseManager.Database.Seeding.Context;
using ExpenseManager.Database.Seeding.Context.SeedingContextMigrations;
using System.Data.Entity;

namespace ExpenseManager.Database.Seeding.Initializers
{
    internal class SeedingInitializer : MigrateDatabaseToLatestVersion<SeedingContext, SeedingContextConfiguration>
    {
    }
}
