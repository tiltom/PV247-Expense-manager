using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.infrastructure;
using ExpenseManager.Entity.Providers.Queryable;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.Database.Contexts
{
    internal class TransactionContext : CurrencyContext, ITransactionContext, ITransactionsProvider
    {
        public TransactionContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<RepeatableTransaction> RepeatableTransactions { get; set; }

        IQueryable<UserProfile> IUserProfilesProvider.UserProfiles
        {
            get { return UserProfiles; }
        }

        public void AttachTransaction(Transaction transaction)
        {
            Transactions.Attach(transaction);
        }

        IQueryable<Transaction> ITransactionsProvider.Transactions
        {
            get { return Transactions; }
        }

        IQueryable<Category> ICategoriesProvider.Categories
        {
            get { return Categories; }
        }

        IQueryable<Wallet> IWalletsQueryable.Wallets
        {
            get { return Wallets; }
        }

        IQueryable<RepeatableTransaction> IRepeatableTransactionsProvider.RepeatableTransactions
        {
            get { return RepeatableTransactions; }
        }

        public IQueryable<WalletAccessRight> WalletAccessRights
        {
            get { throw new NotImplementedException(); }
        }

        IQueryable<Budget> IBudgetsProvider.Budgets
        {
            get { return Budgets; }
        }

        public IQueryable<BudgetAccessRight> BudgetAccessRights
        {
            get { throw new NotImplementedException(); }
        }

        public async Task<bool> AddOrUpdateAsync(Transaction entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingTransaction = entity.Guid == Guid.Empty
                ? null
                : await Transactions.FindAsync(entity.Guid);

            Transactions.AddOrUpdate(x => x.Guid, entity);

            await this.SaveChangesAsync();
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

            await this.SaveChangesAsync();
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

            await this.SaveChangesAsync();
            return existingCategory == null;
        }

        public async Task<DeletedEntity<Category>> DeteleAsync(Category entity)
        {
            var categoryToDelete = entity.Guid == Guid.Empty
                ? null
                : await Categories.FindAsync(entity.Guid);

            Transactions.RemoveRange(categoryToDelete.Transactions);

            var deletedCategory = categoryToDelete == null
                ? null
                : Categories.Remove(categoryToDelete);

            await this.SaveChangesAsync();
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

            await this.SaveChangesAsync();
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

            await this.SaveChangesAsync();
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

            await this.SaveChangesAsync();
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

            await this.SaveChangesAsync();
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

        public Task<bool> AddOrUpdateAsync(UserProfile entity)
        {
            throw new NotImplementedException();
        }

        public Task<DeletedEntity<UserProfile>> DeteleAsync(UserProfile entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddOrUpdateAsync(Budget entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingBudget = entity.Guid == Guid.Empty
                ? null
                : await Budgets.FindAsync(entity.Guid);

            Budgets.AddOrUpdate(x => x.Guid, entity);

            await this.SaveChangesAsync();
            return existingBudget == null;
        }

        public async Task<DeletedEntity<Budget>> DeteleAsync(Budget entity)
        {
            var budgetToDelete = entity.Guid == Guid.Empty
                ? null
                : await Budgets.FindAsync(entity.Guid);

            budgetToDelete.Transactions.Clear();
            var deletedBudget = budgetToDelete == null
                ? null
                : Budgets.Remove(budgetToDelete);

            await this.SaveChangesAsync();
            return new DeletedEntity<Budget>(deletedBudget);
        }

        public Task<bool> AddOrUpdateAsync(BudgetAccessRight entity)
        {
            throw new NotImplementedException();
        }

        public Task<DeletedEntity<BudgetAccessRight>> DeteleAsync(BudgetAccessRight entity)
        {
            throw new NotImplementedException();
        }
    }
}