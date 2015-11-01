using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Providers.infrastructure;
using ExpenseManager.Entity.Transactions;

namespace ExpenseManager.Entity.Providers.Factory
{
    internal class EmptyCategoriesProvider : ICategoriesProvider
    {
        public IQueryable<Category> Categories
        {
            get
            {
                return Enumerable.Empty<Category>().AsQueryable();
            }
        }

        public IQueryable<Transaction> Transactions
        {
            get
            {
                return Enumerable.Empty<Transaction>().AsQueryable();
            }
        }

        public async Task<bool> AddOrUpdateAsync(Transaction entity)
        {
            //await Task.CompletedTask;
            return false;
        }

        public async Task<bool> AddOrUpdateAsync(Category entity)
        {
            //await Task.CompletedTask;
            return false;
        }

        public async Task<DeletedEntity<Transaction>> DeteleAsync(Transaction entity)
        {
            //await Task.CompletedTask;
            return new DeletedEntity<Transaction>(null);
        }

        public async Task<DeletedEntity<Category>> DeteleAsync(Category entity)
        {
            //await Task.CompletedTask;
            return new DeletedEntity<Category>(null);
        }
    }
}
