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
        private static Func<ICategoriesProvider> categoriesProviderCreator;
        private static Func<IUserProfilesProvider> userProfilesProviderCreator;
        private static Func<IWalletsProvider> walletsProviderCreator;

        public static void RegisterBudgetsProvider<TProvider>()
            where TProvider : IBudgetsProvider, new()
        {
            budgetsProviderCreator = () => new TProvider();
        }

        public static void RegisterCategoriesProvider<TProvider>()
            where TProvider : ICategoriesProvider, new()
        {
            categoriesProviderCreator = () => new TProvider();
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

        public static ICategoriesProvider GetNewSelectionsProviders()
        {
            if (categoriesProviderCreator == null)
                return new EmptyCategoriesProvider();

            return categoriesProviderCreator();
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
