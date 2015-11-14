using System.Linq;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Providers.infrastructure;

namespace ExpenseManager.Entity.Providers
{
    public interface ICurrenciesProvider : IAddOrUpdateDeleteEntityProvider<Currency>
    {
        IQueryable<Currency> Currencies { get; }
    }
}