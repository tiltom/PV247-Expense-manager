using System;

namespace ExpenseManager.Entity.Providers.Factory
{
    public static class ProvidersFactory
    {
        private static Func<IBudgetsProvider> _budgetsProviderCreator;
        private static Func<ITransactionsProvider> _transactionsProviderCreator;
        private static Func<IUserProfilesProvider> _userProfilesProviderCreator;
        private static Func<IWalletsProvider> _walletsProviderCreator;

        public static void RegisterBudgetsProvider<TProvider>()
            where TProvider : IBudgetsProvider, new()
        {
            _budgetsProviderCreator = () => new TProvider();
        }

        public static void RegisterTrancastionsProvider<TProvider>()
            where TProvider : ITransactionsProvider, new()
        {
            _transactionsProviderCreator = () => new TProvider();
        }

        public static void RegisterUserProfilesProvider<TProvider>()
            where TProvider : IUserProfilesProvider, new()
        {
            _userProfilesProviderCreator = () => new TProvider();
        }

        public static void RegisterWalletsProvider<TProvider>()
            where TProvider : IWalletsProvider, new()
        {
            _walletsProviderCreator = () => new TProvider();
        }

        public static IBudgetsProvider GetNewBudgetsProviders()
        {
            if (_budgetsProviderCreator == null)
                return new EmptyBudgetsProvider();

            return _budgetsProviderCreator();
        }

        public static ITransactionsProvider GetNewTransactionsProviders()
        {
            if (_transactionsProviderCreator == null)
                return new EmptyTransactionsProvider();

            return _transactionsProviderCreator();
        }

        public static IUserProfilesProvider GetNewUserProfileProviders()
        {
            if (_userProfilesProviderCreator == null)
                return new EmptyUserProfileProvider();

            return _userProfilesProviderCreator();
        }

        public static IWalletsProvider GetNewWalletsProviders()
        {
            if (_walletsProviderCreator == null)
                return new EmptyWalletsProvider();

            return _walletsProviderCreator();
        }
    }
}