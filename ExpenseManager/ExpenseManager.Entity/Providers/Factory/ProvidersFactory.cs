using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.Entity.Providers.Factory
{
    public static class ProvidersFactory
    {
        private static Func<IBudgetsProvider> budgetsProviderCreator;
        private static Func<ITransactionsProvider> transactionsProviderCreator;
        private static Func<IUserProfilesProvider> userProfilesProviderCreator;
        private static Func<IWalletsProvider> walletsProviderCreator;

        public static void RegisterBudgetsProvider<TProvider>()
            where TProvider : IBudgetsProvider, new()
        {
            budgetsProviderCreator = () => new TProvider();
        }

        public static void RegisterTrancastionsProvider<TProvider>()
            where TProvider : ITransactionsProvider, new()
        {
            transactionsProviderCreator = () => new TProvider();
        }

        public static void UserProfilesProvider<TProvider>()
            where TProvider : IUserProfilesProvider, new()
        {
            userProfilesProviderCreator = () => new TProvider();
        }
        public static void WalletsProvider<TProvider>()
           where TProvider : IWalletsProvider, new()
        {
            walletsProviderCreator = () => new TProvider();
        }

        public static IBudgetsProvider GetNewBudgetsProviders()
        {
            if (budgetsProviderCreator == null)
                return new EmptyBudgetsProvider();

            return budgetsProviderCreator();
        }

        public static ITransactionsProvider GetNewTransactionsProviders()
        {
            if (transactionsProviderCreator == null)
                return new EmptyTransactionsProvider();

            return transactionsProviderCreator();
        }

        public static IUserProfilesProvider GetNewUserProfileProviders()
        {
            if (userProfilesProviderCreator == null)
                return new EmptyUserProfileProvider();

            return userProfilesProviderCreator();
        }

        public static IWalletsProvider GetWalletsProviders()
        {
            if (walletsProviderCreator == null)
                return new EmptyWalletsProvider();

            return walletsProviderCreator();
        }
    }
}
