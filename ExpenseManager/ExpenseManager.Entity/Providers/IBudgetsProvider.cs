using System.Linq;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Providers.infrastructure;

namespace ExpenseManager.Entity.Providers
{
    public interface IBudgetsProvider : IAddOrUpdateDeleteEntityProvider<Budget>,
        IBudgetAccessRightsProvider, IUserProfilesProvider
    {
        IQueryable<Budget> Budgets { get; }
    }
}