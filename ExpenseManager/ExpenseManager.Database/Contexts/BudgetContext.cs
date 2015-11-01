using System.Data.Entity;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Transactions;

namespace ExpenseManager.Database.Contexts
{
    internal class BudgetContext : DbContext, IBudgetContext
    {
        public BudgetContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<BudgetAccessRight> BudgetAccessRights { get; set; }

        public DbSet<Budget> Budgets { get; set; }

        public DbSet<Currency> Currencies { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
