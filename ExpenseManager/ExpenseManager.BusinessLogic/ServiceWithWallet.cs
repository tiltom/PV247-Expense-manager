using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.BusinessLogic
{
    public abstract class ServiceWithWallet
    {
        protected abstract IWalletsProvider WalletsProvider { get; }


        /// <summary>
        ///     Returns a wallet by specified ID
        /// </summary>
        /// <param name="id">ID of returned wallet</param>
        /// <returns>Desired wallet</returns>
        public async Task<Wallet> GetWalletById(Guid id)
        {
            return await WalletsProvider.Wallets.Where(w => w.Guid.Equals(id)).FirstOrDefaultAsync();
        }
    }
}