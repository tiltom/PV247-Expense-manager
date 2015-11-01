using ExpenseManager.Entity.Providers.infrastructure;
using ExpenseManager.Entity.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.Entity.Providers
{
    public interface IWalletsProvider : IAddOrUpdateDeleteEntityProvider<Wallet>, IWalletAccessRightsProvider, 
        ITransactionsProvider
    {
        IQueryable<Wallet> Wallets { get; }
    }
}
