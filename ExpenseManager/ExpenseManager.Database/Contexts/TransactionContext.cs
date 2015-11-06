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
using System.Data.Entity.Migrations;

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

        IQueryable<Category> ICategoriesProvider.Categories
        {
            get
            {
                return Categories;
            }
        }

        IQueryable<Wallet> IWalletsProvider.Wallets
        {
            get
            {
                return Wallets;
            }
        }

        IQueryable<RepeatableTransaction> IRepeatableTransactionsProvider.RepeatableTransactions
        {
            get
            {
                return RepeatableTransactions;
            }
        }

        public IQueryable<WalletAccessRight> WalletAccessRights
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public async Task<bool> AddOrUpdateAsync(Transaction entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingTransaction = entity.Guid == Guid.Empty
                ? null
                : await Transactions.FindAsync(entity.Guid);

            Transactions.AddOrUpdate(x => x.Guid, entity);

            return existingTransaction == null;
        }

        public async Task<DeletedEntity<Transaction>> DeteleAsync(Transaction entity)
        {
            var transactionToDelete = entity.Guid == Guid.Empty
                ? null
                : await Transactions.FindAsync(entity.Guid);

            var deletedTransaction = transactionToDelete == null
                ? null
                : Transactions.Remove(transactionToDelete);

            return new DeletedEntity<Transaction>(deletedTransaction);
        }

        public async Task<bool> AddOrUpdateAsync(Category entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingCategory = entity.Guid == Guid.Empty
                ? null
                : await Categories.FindAsync(entity.Guid);

            Categories.AddOrUpdate(x => x.Guid, entity);

            return existingCategory == null;
        }

        public async Task<DeletedEntity<Category>> DeteleAsync(Category entity)
        {
            var categoryToDelete = entity.Guid == Guid.Empty
                ? null
                : await Categories.FindAsync(entity.Guid);

            categoryToDelete.Transactions.Clear();
            var deletedCategory = categoryToDelete == null
                ? null
                : Categories.Remove(categoryToDelete);

            return new DeletedEntity<Category>(deletedCategory);
        }

        public async Task<bool> AddOrUpdateAsync(Wallet entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingWallet = entity.Guid == Guid.Empty
                ? null
                : await Wallets.FindAsync(entity.Guid);

            Wallets.AddOrUpdate(x => x.Guid, entity);

            return existingWallet == null;
        }

        public async Task<DeletedEntity<Wallet>> DeteleAsync(Wallet entity)
        {
            var walletToDelete = entity.Guid == Guid.Empty
                ? null
                : await Wallets.FindAsync(entity.Guid);

            walletToDelete.Transactions.Clear();
            walletToDelete.WalletAccessRights.Clear();
            var deletedWallet = walletToDelete == null
                ? null
                : Wallets.Remove(walletToDelete);

            return new DeletedEntity<Wallet>(deletedWallet);
        }

        public async Task<bool> AddOrUpdateAsync(RepeatableTransaction entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingRepeatableTransaction = entity.Guid == Guid.Empty
                ? null
                : await RepeatableTransactions.FindAsync(entity.Guid);

            RepeatableTransactions.AddOrUpdate(x => x.Guid, entity);

            return existingRepeatableTransaction == null;
        }

        public async Task<DeletedEntity<RepeatableTransaction>> DeteleAsync(RepeatableTransaction entity)
        {
            var RepeatableTransactionToDelete = entity.Guid == Guid.Empty
                ? null
                : await RepeatableTransactions.FindAsync(entity.Guid);
            
            var deletedRepeatableTransaction = RepeatableTransactionToDelete == null
                ? null
                : RepeatableTransactions.Remove(RepeatableTransactionToDelete);

            return new DeletedEntity<RepeatableTransaction>(deletedRepeatableTransaction);
        }

        public Task<bool> AddOrUpdateAsync(WalletAccessRight entity)
        {
            throw new NotImplementedException();
        }

        public Task<DeletedEntity<WalletAccessRight>> DeteleAsync(WalletAccessRight entity)
        {
            throw new NotImplementedException();
        }
    }
}
