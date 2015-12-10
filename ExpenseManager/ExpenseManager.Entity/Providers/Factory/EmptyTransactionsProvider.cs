using System;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Providers.infrastructure;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.Entity.Providers.Factory
{
    internal class EmptyTransactionsProvider : ITransactionsProvider
    {
        public IQueryable<BudgetAccessRight> BudgetAccessRights
        {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<Budget> Budgets
        {
            get { return Enumerable.Empty<Budget>().AsQueryable(); }
        }

        public IQueryable<Category> Categories
        {
            get { return Enumerable.Empty<Category>().AsQueryable(); }
        }

        public IQueryable<Currency> Currencies
        {
            get { return Enumerable.Empty<Currency>().AsQueryable(); }
        }

        public IQueryable<RepeatableTransaction> RepeatableTransactions
        {
            get { return Enumerable.Empty<RepeatableTransaction>().AsQueryable(); }
        }

        public IQueryable<Transaction> Transactions
        {
            get { return Enumerable.Empty<Transaction>().AsQueryable(); }
        }

        public void AttachTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public IQueryable<UserProfile> UserProfiles
        {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<WalletAccessRight> WalletAccessRights
        {
            get { return Enumerable.Empty<WalletAccessRight>().AsQueryable(); }
        }

        public IQueryable<Wallet> Wallets
        {
            get { return Enumerable.Empty<Wallet>().AsQueryable(); }
        }

        public async Task<bool> AddOrUpdateAsync(WalletAccessRight entity)
        {
            await Task.CompletedTask;
            return false;
        }

        public Task<bool> AddOrUpdateAsync(UserProfile entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddOrUpdateAsync(Budget entity)
        {
            await Task.CompletedTask;
            return false;
        }

        public Task<bool> AddOrUpdateAsync(BudgetAccessRight entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddOrUpdateAsync(Currency entity)
        {
            await Task.CompletedTask;
            return false;
        }

        public async Task<bool> AddOrUpdateAsync(RepeatableTransaction entity)
        {
            await Task.CompletedTask;
            return false;
        }

        public async Task<bool> AddOrUpdateAsync(Wallet entity)
        {
            await Task.CompletedTask;
            return false;
        }

        public async Task<bool> AddOrUpdateAsync(Category entity)
        {
            await Task.CompletedTask;
            return false;
        }

        public async Task<bool> AddOrUpdateAsync(Transaction entity)
        {
            await Task.CompletedTask;
            return false;
        }

        public async Task<DeletedEntity<Currency>> DeteleAsync(Currency entity)
        {
            await Task.CompletedTask;
            return new DeletedEntity<Currency>(null);
        }

        public Task<DeletedEntity<BudgetAccessRight>> DeteleAsync(BudgetAccessRight entity)
        {
            throw new NotImplementedException();
        }

        public async Task<DeletedEntity<Budget>> DeteleAsync(Budget entity)
        {
            await Task.CompletedTask;
            return new DeletedEntity<Budget>(null);
        }

        public Task<DeletedEntity<UserProfile>> DeteleAsync(UserProfile entity)
        {
            throw new NotImplementedException();
        }

        public async Task<DeletedEntity<RepeatableTransaction>> DeteleAsync(RepeatableTransaction entity)
        {
            await Task.CompletedTask;
            return new DeletedEntity<RepeatableTransaction>(null);
        }

        public async Task<DeletedEntity<WalletAccessRight>> DeteleAsync(WalletAccessRight entity)
        {
            await Task.CompletedTask;
            return new DeletedEntity<WalletAccessRight>(null);
        }

        public async Task<DeletedEntity<Wallet>> DeteleAsync(Wallet entity)
        {
            await Task.CompletedTask;
            return new DeletedEntity<Wallet>(null);
        }

        public async Task<DeletedEntity<Category>> DeteleAsync(Category entity)
        {
            await Task.CompletedTask;
            return new DeletedEntity<Category>(null);
        }

        public async Task<DeletedEntity<Transaction>> DeteleAsync(Transaction entity)
        {
            await Task.CompletedTask;
            return new DeletedEntity<Transaction>(null);
        }
    }
}