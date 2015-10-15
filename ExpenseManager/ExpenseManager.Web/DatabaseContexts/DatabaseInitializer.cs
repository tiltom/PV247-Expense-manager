using System.Data.Entity;

namespace ExpenseManager.Web.DatabaseContexts
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            Database.SetInitializer(new ExpenseManagerInitializater());
            var context = new ExpenseManagerContext();
            context.Database.Initialize(true);
        }
    }
}