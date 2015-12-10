using System.Linq;
using ExpenseManager.Entity.Providers.infrastructure;
using ExpenseManager.Entity.Transactions;

namespace ExpenseManager.Entity.Providers
{
    public interface ITransactionsProvider : IAddOrUpdateDeleteEntityProvider<Transaction>, ICategoriesProvider,
        IWalletsProvider, IRepeatableTransactionsProvider, ICurrenciesProvider, IBudgetsProvider
    {
        IQueryable<Transaction> Transactions { get; }

        void AttachTransaction(Transaction transaction);
    }
}