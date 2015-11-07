using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExpenseManager.Web.DatabaseContexts;
using ExpenseManager.Web.Models.Wallet;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Factory;

namespace ExpenseManager.Web.Controllers
{
    [Authorize]
    public class WalletController : AbstractController
    {
        private readonly IWalletsProvider db = ProvidersFactory.GetNewWalletsProviders();

        /// <summary>
        ///     Address: GET: Wallets/Edit
        ///     Show edit wallet page for current UserProfile
        /// </summary>
        /// <returns> page for currently logged UserProfile or HttpNotFound if no wallet was created</returns>
        public async Task<ActionResult> Edit()
        {
            // get UserProfile and his wallet from context
            var id = await this.CurrentProfileId();
            var wallet = await this.db.Wallets.Where(u => u.Owner.Guid == id).FirstOrDefaultAsync();
            if (wallet == null)
            {
                return this.HttpNotFound();
            }
            return this.View(new WalletEditModel
            {
                Guid = wallet.Guid,
                CurrencyId = wallet.Currency.Guid,
                Name = wallet.Name,
                Currencies = await this.GetCurrencies()
            });
        }

        /// <summary>
        ///     Address: POST: Wallets/Edit
        ///     Edit existing wallet
        /// </summary>
        /// <param name="wallet"> model of wallet posted from UserProfile</param>
        /// <returns>In case of everything filled correctly - redirect to UserProfile management, otherwise - same page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(WalletEditModel wallet)
        {
            if (ModelState.IsValid)
            {
                var walletEntity = this.db.Wallets.Where(w => w.Guid.Equals(wallet.Guid)).FirstOrDefaultAsync().Result;
                walletEntity.Owner = walletEntity.Owner;
                walletEntity.Currency = this.db.Currencies.Where(w => w.Guid.Equals(wallet.CurrencyId)).FirstOrDefaultAsync().Result;
                walletEntity.Name = wallet.Name;
                return this.RedirectToAction("Index", "Manage");
            }
            wallet.Currencies = await this.GetCurrencies();
            return this.View(wallet);
        }

        #region protected

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //this.db.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region private

        private async Task<List<SelectListItem>> GetCurrencies()
        {
            var currencies = await this.db.Currencies.Select(currency => new SelectListItem
            {
                Text = currency.Name,
                Value = currency.Guid.ToString()
            }).ToListAsync();
            return currencies;
        }

        #endregion
    }
}