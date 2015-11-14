using System.Linq;
using ExpenseManager.Entity.Providers.infrastructure;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.Entity.Providers
{
    public interface IWalletsProvider : IAddOrUpdateDeleteEntityProvider<Wallet>, IWalletAccessRightsProvider,
        IUserProfilesProvider, ICurrenciesProvider
    {
        IQueryable<Wallet> Wallets { get; }
    }
}