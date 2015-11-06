using ExpenseManager.Entity.Providers.infrastructure;
using ExpenseManager.Entity.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.Entity.Providers
{
    public interface ITransactionsProvider : IAddOrUpdateDeleteEntityProvider<Transaction>, ICategoriesProvider, 
        IWalletsProvider, IRepeatableTransactionsProvider
    {
        IQueryable<Transaction> Transactions { get; }
    }
}
