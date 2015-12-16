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
            //Drop everything before seeding otherwise we will have duplicitous data
            context.Database.ExecuteSqlCommand("sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
            context.Database.ExecuteSqlCommand(
                "sp_MSForEachTable 'IF OBJECT_ID(''?'') NOT IN (ISNULL(OBJECT_ID(''[dbo].[__MigrationHistory]''),0)) DELETE FROM ?'");
            context.Database.ExecuteSqlCommand("EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'");

            new CurrenciesSeed<SeedingContext>().Seed(context);
            new UsersSeed<SeedingContext>().Seed(context);
            new CategoriesSeed<SeedingContext>().Seed(context);
            new TransactionsSeed<SeedingContext>().Seed(context);
            new RepeatableTransactionsSeed<SeedingContext>().Seed(context);
            new BudgetsSeed<SeedingContext>().Seed(context);
            new BudgetAccessRightsSeed<SeedingContext>().Seed(context);
        }
    }
}