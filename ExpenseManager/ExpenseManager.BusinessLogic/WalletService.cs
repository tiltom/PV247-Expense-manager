using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.BusinessLogic
{
    /// <summary>
    ///     Class that handles logic of WalletController
    /// </summary>
    public class WalletService
    {
        private readonly IWalletsProvider _db = ProvidersFactory.GetNewWalletsProviders();

        /// <summary>
        ///     Returns wallet owner by owner's ID
        /// </summary>
        /// <param name="id">ID of wallet owner</param>
        /// <returns>Wallet owner</returns>
        public IQueryable GetWalletByOwnerId(Guid id)
        {
            return this._db.Wallets.Where(u => u.Owner.Guid.Equals(id));
        }

        /// <summary>
        ///     Returns a wallet by specified ID
        /// </summary>
        /// <param name="id">ID of returned wallet</param>
        /// <returns>Desired wallet</returns>
        public async Task<Wallet> GetWalletById(Guid id)
        {
            return await this._db.Wallets.Where(w => w.Guid.Equals(id)).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Edits specifiet wallet
        /// </summary>
        /// <param name="id">ID of changed wallet</param>
        /// <param name="name">New name of wallet</param>
        /// <param name="currencyId">ID of new currency</param>
        /// <returns></returns>
        public async Task EditWallet(Guid id, string name, Guid currencyId)
        {
            var wallet = await this.GetWalletById(id);
            var currency = await this.GetCurrencyById(currencyId);

            wallet.Name = name;
            wallet.Currency = currency;

            await this._db.AddOrUpdateAsync(wallet);
        }

        /// <summary>
        ///     Returns all currencies from database
        /// </summary>
        /// <returns>All currencies</returns>
        public IQueryable<Currency> GetCurrencies()
        {
            return this._db.Currencies;
        }

        #region private

        private async Task<Currency> GetCurrencyById(Guid id)
        {
            return await this._db.Currencies.Where(x => x.Guid.Equals(id)).FirstOrDefaultAsync();
        }

        #endregion
    }
}