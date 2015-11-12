using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Wallets;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.Database.Contexts
{
    internal interface ITransactionContext : ICurrencyContext
    {
        DbSet<Transaction> Transactions { get; set; }
        DbSet<Wallet> Wallets { get; set; }
        DbSet<Budget> Budgets { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<RepeatableTransaction> RepeatableTransactions { get; set; }
    }
}
