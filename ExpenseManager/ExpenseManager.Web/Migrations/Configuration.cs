using System.Data.Entity.Migrations;
using ExpenseManager.Web.DatabaseContexts;

namespace ExpenseManager.Web.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
    }
}