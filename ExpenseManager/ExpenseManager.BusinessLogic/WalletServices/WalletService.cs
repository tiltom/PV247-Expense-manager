using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.BusinessLogic.ExchangeRates;
using ExpenseManager.BusinessLogic.Validators;
using ExpenseManager.BusinessLogic.Validators.Extensions;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Queryable;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.BusinessLogic.WalletServices
{
    /// <summary>
    ///     Class that handles logic of WalletController
    /// </summary>
    public class WalletService : ServiceWithWallet, IServiceValidation<Wallet>
    {
        private readonly IWalletsProvider _db;
        private readonly WalletValidator _validator;

        public WalletService(IWalletsProvider db)
        {
            this._db = db;
            this._validator = new WalletValidator();
        }

        protected override IWalletsQueryable WalletsProvider
        {
            get { return this._db; }
        }

        /// <summary>
        ///     Validates wallet
        /// </summary>
        /// <param name="wallet">Wallet to validate</param>
        /// <returns>True if wallet is valid, false otherwise</returns>
        public void Validate(Wallet wallet)
        {
            if (wallet == null)
                throw new ArgumentNullException(nameof(wallet));

            this._validator.ValidateAndThrowCustomException(wallet);
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

            this.Validate(wallet);
            foreach (var transaction in wallet.Transactions)
            {
                Transformation.ChangeCurrency(transaction, currency);
            }
            // TODO change budget limit currency
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