using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper.QueryableExtensions;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Web.Models.Wallet;

namespace ExpenseManager.Web.Controllers
{
    [Authorize]
    public class WalletController : AbstractController
    {
        private readonly IWalletsProvider _db = ProvidersFactory.GetNewWalletsProviders();


        /// <summary>
        ///     Address: GET: Wallets/Edit
        ///     Show edit wallet page for current UserProfile
        /// </summary>
        /// <returns> page for currently logged UserProfile or HttpNotFound if no wallet was created</returns>
        public async Task<ActionResult> Edit()
        {
            // get UserProfile and his wallet from context
            var id = await this.CurrentProfileId();
            var walletEditModel =
                await this._db.Wallets
                    .Where(u => u.Owner.Guid == id)
                    .ProjectTo<WalletEditModel>()
                    .FirstOrDefaultAsync();
            if (walletEditModel == null)
            {
                return this.HttpNotFound();
            }
            walletEditModel.Currencies = await this.GetCurrencies();
            return this.View(walletEditModel);
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
                var walletEntity = await this._db.Wallets.Where(w => w.Guid == wallet.Guid).FirstOrDefaultAsync();
                walletEntity.Owner = walletEntity.Owner;
                walletEntity.Currency =
                    await this._db.Currencies.Where(c => c.Guid == wallet.CurrencyId).FirstOrDefaultAsync();
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
                // TODO commented out ? why ?
                //this.db.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region private

        private async Task<List<SelectListItem>> GetCurrencies()
        {
            var currencies = await this._db.Currencies.Select(currency => new SelectListItem
            {
                Text = currency.Name,
                Value = currency.Guid.ToString()
            }).ToListAsync();
            return currencies;
        }

        #endregion
    }
}