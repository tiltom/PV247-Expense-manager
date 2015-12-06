using ExpenseManager.Entity.Providers.infrastructure;
using ExpenseManager.Entity.Providers.Queryable;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.Entity.Providers
{
    public interface IWalletsProvider : IAddOrUpdateDeleteEntityProvider<Wallet>, IWalletAccessRightsProvider,
        IUserProfilesProvider, ICurrenciesProvider, IWalletsQueryable
    {
    }
}