using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Wallets;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseManager.Entity.Providers.infrastructure;

namespace ExpenseManager.Database.Contexts
{
    internal class TransactionContext : DbContext, ITransactionContext, ITransactionsProvider
    {
        public TransactionContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<RepeatableTransaction> RepeatableTransactions { get; set; }

        IQueryable<Transaction> ITransactionsProvider.Transactions
        {
            get
            {
                return Transactions;
            }
        }

        public Task<bool> AddOrUpdateAsync(Transaction entity)
        {
            throw new NotImplementedException();
        }

        public Task<DeletedEntity<Transaction>> DeteleAsync(Transaction entity)
        {
            throw new NotImplementedException();
        }
    }
}
