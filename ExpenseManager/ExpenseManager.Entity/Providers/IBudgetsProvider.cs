using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Providers.infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.Entity.Providers
{
    public interface IBudgetsProvider : IAddOrUpdateDeleteEntityProvider<Budget>, ITransactionsProvider,
        IBudgetAccessRightsProvider
    {
        IQueryable<Budget> Budgets { get; }
    }
}
