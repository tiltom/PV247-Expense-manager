using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Providers.infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.Entity.Providers
{
    public interface ICurrenciesProvider : IAddOrUpdateDeleteEntityProvider<Currency>
    {
        IQueryable<Currency> Currencies { get; }
    }
}
