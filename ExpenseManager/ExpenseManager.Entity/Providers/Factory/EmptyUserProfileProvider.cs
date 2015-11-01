using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Providers.infrastructure;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.Entity.Providers.Factory
{
    internal class EmptyUserProfileProvider : IUserProfilesProvider
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

        public IQueryable<WalletAccessRight> WalletAccessRight
        {
            get
            {
                return Enumerable.Empty<WalletAccessRight>().AsQueryable();
            }
        }

        public async Task<bool> AddOrUpdateAsync(Budget entity)
        {
            //await Task.CompletedTask;
            return false;
        }

        public async Task<bool> AddOrUpdateAsync(BudgetAccessRight entity)
        {
            //await Task.CompletedTask;
            return false;
        }

        public async Task<bool> AddOrUpdateAsync(WalletAccessRight entity)
        {
            //await Task.CompletedTask;
            return false;
        }

        public async Task<bool> AddOrUpdateAsync(Transaction entity)
        {
            //await Task.CompletedTask;
            return false;
        }

        public async Task<bool> AddOrUpdateAsync(UserProfile entity)
        {
            //await Task.CompletedTask;
            return false;
        }

        public async Task<DeletedEntity<BudgetAccessRight>> DeteleAsync(BudgetAccessRight entity)
        {
            //await Task.CompletedTask;
            return new DeletedEntity<BudgetAccessRight>(null);
        }

        public async Task<DeletedEntity<Budget>> DeteleAsync(Budget entity)
        {
            //await Task.CompletedTask;
            return new DeletedEntity<Budget>(null);
        }

        public async Task<DeletedEntity<Transaction>> DeteleAsync(Transaction entity)
        {
            //await Task.CompletedTask;
            return new DeletedEntity<Transaction>(null);
        }

        public async Task<DeletedEntity<WalletAccessRight>> DeteleAsync(WalletAccessRight entity)
        {
            //await Task.CompletedTask;
            return new DeletedEntity<WalletAccessRight>(null);
        }

        public async Task<DeletedEntity<UserProfile>> DeteleAsync(UserProfile entity)
        {
            //await Task.CompletedTask;
            return new DeletedEntity<UserProfile>(null);
        }
    }
}
