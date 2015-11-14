using System.Data.Entity.Migrations;
using ExpenseManager.Database.Seeding.Seeds;

namespace ExpenseManager.Database.Seeding.Context.SeedingContextMigrations
{
    internal sealed class SeedingContextConfiguration : DbMigrationsConfiguration<SeedingContext>
    {
        public SeedingContextConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            MigrationsDirectory = @"Contexts\SeedingContextMigrations";
        }

        protected override void Seed(SeedingContext context)
        {
            new CurrenciesSeed<SeedingContext>().Seed(context);
            new CategoriesSeed<SeedingContext>().Seed(context);
            new UsersSeed<SeedingContext>().Seed(context);
            new TransactionsSeed<SeedingContext>().Seed(context);
            new RepeatableTransactionsSeed<SeedingContext>().Seed(context);
            new BudgetsSeed<SeedingContext>().Seed(context);
            new BudgetAccessRightsSeed<SeedingContext>().Seed(context);
        }
    }
}