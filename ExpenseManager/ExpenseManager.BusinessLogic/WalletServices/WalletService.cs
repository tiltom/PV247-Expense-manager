using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.BusinessLogic.ExchangeRates;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.BusinessLogic.WalletServices
{
    /// <summary>
    ///     Class that handles logic of WalletController
    /// </summary>
    public class WalletService : ServiceWithWallet
    {
        private readonly IWalletsProvider _db;

        public WalletService(IWalletsProvider db)
        {
            this._db = db;
        }

        protected override IWalletsProvider WalletsProvider
        {
            get { return this._db; }
        }


        /// <summary>
        ///     Returns wallet owner by owner's ID
        /// </summary>
        /// <param name="id">ID of wallet owner</param>
        /// <returns>Wallet owner</returns>
        public IQueryable<Wallet> GetWalletByOwnerId(Guid id)
        {
            return
                this._db.WalletAccessRights.Where(
                    war => war.Permission == PermissionEnum.Owner
                           && war.UserProfile.Guid == id
                    )
                    .Select(war => war.Wallet);
        }


        /// <summary>
        ///     Edits specified wallet
        /// </summary>
        /// <param name="id">ID of changed wallet</param>
        /// <param name="name">New name of wallet</param>
        /// <param name="currencyId">ID of new currency</param>
        /// <returns></returns>
        public async Task EditWallet(Guid id, string name, Guid currencyId)
        {
            var wallet = await this.GetWalletById(id);
            var oldCurrency = wallet.Currency;
            var currency = await this.GetCurrencyById(currencyId);

            wallet.Name = name;
            wallet.Currency = currency;

            if (this.ValidateWallet(wallet))
            {
                foreach (var transaction in wallet.Transactions)
                {
                    Transformation.ChangeCurrency(transaction, currency);
                }
                // TODO change budget limit currency
                await this._db.AddOrUpdateAsync(wallet);
            }
        }

        /// <summary>
        ///     Returns all currencies from database
        /// </summary>
        /// <returns>All currencies</returns>
        public IQueryable<Currency> GetCurrencies()
        {
            return this._db.Currencies;
        }

        /// <summary>
        ///     Validates wallet
        /// </summary>
        /// <param name="wallet">Wallet to validate</param>
        /// <returns>True if wallet is valid, false otherwise</returns>
        public bool ValidateWallet(Wallet wallet)
        {
            if (wallet?.Currency == null)
            {
                return false;
            }

            if (wallet.Name.Equals(string.Empty))
            {
                return false;
            }

            return true;
        }

        #region private

        private async Task<Currency> GetCurrencyById(Guid id)
        {
            return await this._db.Currencies.Where(x => x.Guid.Equals(id)).FirstOrDefaultAsync();
        }

        #endregion
    }
}