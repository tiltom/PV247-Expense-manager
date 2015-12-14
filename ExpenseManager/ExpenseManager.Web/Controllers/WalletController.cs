using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper.QueryableExtensions;
using ExpenseManager.BusinessLogic;
using ExpenseManager.BusinessLogic.WalletServices;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Resources;
using ExpenseManager.Resources.WalletResources;
using ExpenseManager.Web.Constants;
using ExpenseManager.Web.Helpers;
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
            if (!ModelState.IsValid)
            {
                this.AddError(SharedResource.ModelStateIsNotValid);
                wallet.Currencies = await this.GetCurrencies();
                return this.View(wallet);
            }

            try
            {
                await this._walletService.EditWallet(wallet.Guid, wallet.Name, wallet.CurrencyId);
            }
            catch (ServiceValidationException exception)
            {
                ModelState.AddModelErrors(exception);
                wallet.Currencies = await this.GetCurrencies();
                return this.View(wallet);
            }

            this.AddSuccess(WalletResource.WalletEdited);
            return this.RedirectToAction(SharedResource.Index, SharedConstant.DashBoard);
        }
    }
}