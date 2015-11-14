using System.Linq;
using ExpenseManager.Entity.Providers.infrastructure;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.Entity.Providers
{
    public interface IWalletAccessRightsProvider : IAddOrUpdateDeleteEntityProvider<WalletAccessRight>
    {
        IQueryable<WalletAccessRight> WalletAccessRights { get; }
    }
}