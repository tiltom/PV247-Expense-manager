using System.Data.Entity;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Users;

namespace ExpenseManager.Database.Contexts
{
    internal interface IBudgetContext : ICurrencyContext
    {
        DbSet<Budget> Budgets { get; set; }
        DbSet<BudgetAccessRight> BudgetAccessRights { get; set; }
        DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<Transaction> Transactions { get; set; }
    }
}