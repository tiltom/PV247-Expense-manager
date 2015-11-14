using System.Linq;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Providers.infrastructure;

namespace ExpenseManager.Entity.Providers
{
    public interface IBudgetAccessRightsProvider : IAddOrUpdateDeleteEntityProvider<BudgetAccessRight>
    {
        IQueryable<BudgetAccessRight> BudgetAccessRights { get; }
    }
}