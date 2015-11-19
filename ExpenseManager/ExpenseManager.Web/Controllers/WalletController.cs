using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper.QueryableExtensions;
using ExpenseManager.BusinessLogic.WalletServices;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Web.Models.Wallet;

namespace ExpenseManager.Web.Controllers
{
    [Authorize]
    public class WalletController : AbstractController
    {
        private readonly WalletService _walletService =
            new WalletService(ProvidersFactory.GetNewWalletsProviders());

        /// <summary>
        ///     Address: GET: Wallets/Edit
        ///     Show edit wallet page for current UserProfile
        /// </summary>
        /// <returns> page for currently logged UserProfile or HttpNotFound if no wallet was created</returns>
        public async Task<ActionResult> Edit()
        {
            // get UserProfile and his wallet
            var id = await this.CurrentProfileId();
            var wallet = this._walletService.GetWalletByOwnerId(id);

            var walletEditModel = await wallet.ProjectTo<WalletEditModel>().FirstOrDefaultAsync();

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
                await this._walletService.EditWallet(wallet.Guid, wallet.Name, wallet.CurrencyId);

                return this.RedirectToAction("Index", "Home");
            }

            wallet.Currencies = await this.GetCurrencies();
            return this.View(wallet);
        }

        #region private

        private async Task<List<SelectListItem>> GetCurrencies()
        {
            var currencies = await this._walletService.GetCurrencies().Select(currency => new SelectListItem
            {
                Text = currency.Name,
                Value = currency.Guid.ToString()
            }).ToListAsync();
            return currencies;
        }

        #endregion
    }
}