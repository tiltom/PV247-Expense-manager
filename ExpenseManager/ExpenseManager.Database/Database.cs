using System.Data.Entity.SqlServer;
using ExpenseManager.Database.Seeding.Initializers;
using DB = System.Data.Entity.Database;

namespace ExpenseManager.Database
{
    /// <summary>
    ///     Provides methods for general operations on ExpenseManager.Database.
    /// </summary>
    public static class Database
    {
        /// <summary>
        ///     Allows correct initialization of ExpenseManager.Database.
        /// </summary>
        public static void Initialize()
        {
            // See for details regards this line: http://stackoverflow.com/a/19130718/1138663
            var instance = SqlProviderServices.Instance;

            DB.SetInitializer(new SeedingInitializer());

            using (var context = new Seeding.Context.SeedingContext())
            {
                context.Database.Initialize(true);
            }
        }
    }
}