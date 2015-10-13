using System.Data.Entity;

namespace ExpenseManager.BusinessLogic.DatabaseContext
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<ExpenseManagerContext>());
            var context = new ExpenseManagerContext();
            context.Database.Initialize(true);
        }
    }
}