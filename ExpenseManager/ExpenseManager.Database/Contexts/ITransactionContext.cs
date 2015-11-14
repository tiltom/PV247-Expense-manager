using System.Data.Entity;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Wallets;

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