using System.Linq;
using ExpenseManager.Entity.Providers.infrastructure;
using ExpenseManager.Entity.Transactions;

namespace ExpenseManager.Entity.Providers
{
    public interface IRepeatableTransactionsProvider : IAddOrUpdateDeleteEntityProvider<RepeatableTransaction>
    {
        IQueryable<RepeatableTransaction> RepeatableTransactions { get; }
    }
}