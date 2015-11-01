using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Users;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.Database.Contexts
{
    internal interface IBudgetContext
    {
        DbSet<Budget> Budgets { get; set; }
        DbSet<BudgetAccessRight> BudgetAccessRights { get; set; }
        DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<Currency> Currencies { get; set; }
        DbSet<Transaction> Transactions { get; set; }
    }
}
