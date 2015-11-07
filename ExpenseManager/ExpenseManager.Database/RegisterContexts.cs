using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity.Providers.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
