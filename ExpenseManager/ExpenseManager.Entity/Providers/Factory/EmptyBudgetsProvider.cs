using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Providers.infrastructure;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.Entity.Providers.Factory
{
    internal class EmptyBudgetsProvider : IBudgetsProvider
    {
        public IQueryable<BudgetAccessRight> BudgetAccessRights
        {
            get
            {
                return Enumerable.Empty<BudgetAccessRight>().AsQueryable();
            }
        }

        public IQueryable<Budget> Budgets
        {
            get
            {
                return Enumerable.Empty<Budget>().AsQueryable();
            }
        }

        public IQueryable<Category> Categories
        {
            get
            {
                return Enumerable.Empty<Category>().AsQueryable();
            }
        }

        public IQueryable<RepeatableTransaction> RepeatableTransactions
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IQueryable<Transaction> Transactions
        {
            get
            {
                return Enumerable.Empty<Transaction>().AsQueryable();
            }
        }

        public IQueryable<UserProfile> UserProfiles
        {
            get
            {
                return Enumerable.Empty<UserProfile>().AsQueryable();
            }
        }

        public IQueryable<WalletAccessRight> WalletAccessRights
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IQueryable<Wallet> Wallets
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public async Task<bool> AddOrUpdateAsync(Category entity)
        {
            await Task.CompletedTask;
            return false;
        }

        public Task<bool> AddOrUpdateAsync(WalletAccessRight entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddOrUpdateAsync(UserProfile entity)
        {
            await Task.CompletedTask;
            return false;
        }

        public Task<bool> AddOrUpdateAsync(RepeatableTransaction entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddOrUpdateAsync(Wallet entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddOrUpdateAsync(BudgetAccessRight entity)
        {
            await Task.CompletedTask;
            return false;
        }

        public async Task<bool> AddOrUpdateAsync(Transaction entity)
        {
            await Task.CompletedTask;
            return false;
        }

        public async Task<bool> AddOrUpdateAsync(Budget entity)
        {
            await Task.CompletedTask;
            return false;
        }

        public Task<DeletedEntity<RepeatableTransaction>> DeteleAsync(RepeatableTransaction entity)
        {
            throw new NotImplementedException();
        }

        public async Task<DeletedEntity<UserProfile>> DeteleAsync(UserProfile entity)
        {
            await Task.CompletedTask;
            return new DeletedEntity<UserProfile>(null);
        }

        public Task<DeletedEntity<WalletAccessRight>> DeteleAsync(WalletAccessRight entity)
        {
            throw new NotImplementedException();
        }

        public Task<DeletedEntity<Wallet>> DeteleAsync(Wallet entity)
        {
            throw new NotImplementedException();
        }

        public async Task<DeletedEntity<Category>> DeteleAsync(Category entity)
        {
            await Task.CompletedTask;
            return new DeletedEntity<Category>(null);
        }

        public async Task<DeletedEntity<BudgetAccessRight>> DeteleAsync(BudgetAccessRight entity)
        {
            await Task.CompletedTask;
            return new DeletedEntity<BudgetAccessRight>(null);
        }

        public async Task<DeletedEntity<Transaction>> DeteleAsync(Transaction entity)
        {
            await Task.CompletedTask;
            return new DeletedEntity<Transaction>(null);
        }

        public async Task<DeletedEntity<Budget>> DeteleAsync(Budget entity)
        {
            await Task.CompletedTask;
            return new DeletedEntity<Budget>(null);
        }
    }
}
