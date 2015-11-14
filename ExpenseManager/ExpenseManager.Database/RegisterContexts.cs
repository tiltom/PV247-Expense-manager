using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity.Providers.Factory;

namespace ExpenseManager.Database
{
    public static class RegisterContexts
    {
        public static void Register()
        {
            ProvidersFactory.RegisterBudgetsProvider<BudgetContext>();
            ProvidersFactory.RegisterTrancastionsProvider<TransactionContext>();
            ProvidersFactory.RegisterWalletsProvider<WalletContext>();
            // TODO REGISTER PROVIDERS
        }
    }
}