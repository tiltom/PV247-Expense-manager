using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Providers.infrastructure;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.Entity.Providers.Factory
{
    internal class EmptyTransactionsProvider : ITransactionsProvider
    {
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
                return Enumerable.Empty<RepeatableTransaction>().AsQueryable();
            }
        }

        public IQueryable<Transaction> Transactions
        {
            get
            {
                return Enumerable.Empty<Transaction>().AsQueryable();
            }
        }

        public IQueryable<WalletAccessRight> WalletAccessRights
        {
            get
            {
                return Enumerable.Empty<WalletAccessRight>().AsQueryable();
            }
        }

        public IQueryable<Wallet> Wallets
        {
            get
            {
                return Enumerable.Empty<Wallet>().AsQueryable();
            }
        }

        public async Task<bool> AddOrUpdateAsync(WalletAccessRight entity)
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
