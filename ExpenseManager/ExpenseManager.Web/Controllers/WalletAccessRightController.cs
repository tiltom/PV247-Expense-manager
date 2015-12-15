using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using CaptchaMvc.HtmlHelpers;
using ExpenseManager.BusinessLogic;
using ExpenseManager.BusinessLogic.WalletServices;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;
using ExpenseManager.Resources;
using ExpenseManager.Resources.WalletResources;
using ExpenseManager.Web.Constants;
using ExpenseManager.Web.Helpers;
using ExpenseManager.Web.Models.WalletAccessRight;
using PagedList;

namespace ExpenseManager.Web.Controllers
{
    [Authorize]
    public class WalletAccessRightController : AbstractController
    {
        private readonly CommonService _commonService = new CommonService();

        private readonly WalletAccessRightService _walletAccessRightService =
            new WalletAccessRightService(ProvidersFactory.GetNewWalletsProviders(), new CommonService());


        /// <summary>
        ///     Returns index of all wallet access rights
        /// </summary>
        /// <returns>paged list of all wallet access rights</returns>
        // GET: WalletAccessRights
        public async Task<ActionResult> Index(int? page)
        {
            var id = await this.CurrentProfileId();

            var accessRights = await this._walletAccessRightService.GetAccessRightsByWalletOwnerId(id);
            var accessRightModels = accessRights.Select(Mapper.Map<WalletAccessRightModel>);
            var pageNumber = page ?? SharedConstant.DefaultStartPage;
            return this.View(accessRightModels.ToPagedList(pageNumber, SharedConstant.PageSize));
        }

        /// <summary>
        ///     Address :  GET: WalletAccessRights/Create
        ///     get the view with combos for editing wallet access rights
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Create()
        {
            var profileId = await this.CurrentProfileId();
            var walletId = await this._walletAccessRightService.GetWalletAccessRightIdByWalletId(profileId);

            return
                this.View(this.ConvertEntityToModelWithComboOptions(new WalletAccessRight
                {
                    Wallet = new Wallet
                    {
                        Guid = walletId
                    },
                    UserProfile = new UserProfile()
                }));
        }

        /// <summary>
        ///     Address : POST: WalletAccessRights/Create
        ///     creates wallet access rights for given parameters
        /// </summary>
        /// <returns>redirect to list with all access rights or same view with error message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(WalletAccessRightModel walletAccessRight)
        {
            this.IsCaptchaValid(SharedResource.CaptchaValidationFailed);
            if (!ModelState.IsValid)
            {
                this.AddError(SharedResource.ModelStateIsNotValid);
                walletAccessRight.Permissions = this.GetPermissions();
                return this.View(walletAccessRight);
            }
            var userId = await this.GetUserProfileByEmail(walletAccessRight.AssignedUserEmail);

            try
            {
                await this._walletAccessRightService.CreateWalletAccessRight(
                    walletAccessRight.WalletId,
                    userId,
                    walletAccessRight.Permission
                    );
            }
            catch (ServiceValidationException exception)
            {
                ModelState.AddModelErrors(exception);
                walletAccessRight.Permissions = this.GetPermissions();
                return this.View(walletAccessRight);
            }

            this.AddSuccess(string.Format(WalletAccessRightResource.RightCreated, walletAccessRight.AssignedUserEmail));
            return this.RedirectToAction(SharedConstant.Index);
        }


        /// <summary>
        ///     Address : GET: WalletAccessRights/Edit/5
        ///     get the view with combos for editing wallet access rights
        /// </summary>
        /// <param name="id">id of wallet access right</param>
        /// <returns>view</returns>
        public async Task<ActionResult> Edit(Guid id)
        {
            var walletAccessRight = await this._walletAccessRightService.GetWalletAccessRightById(id);

            if (walletAccessRight == null)
            {
                return this.HttpNotFound();
            }
            return this.View(this.ConvertEntityToEditModel(walletAccessRight));
        }

        /// <summary>
        ///     Address: POST: WalletAccessRights/Edit/5
        ///     saves the changes in wallet access rights
        /// </summary>
        /// <param name="walletAccessRight"> model of access rights for front end</param>
        /// <returns>redirect to list with all access rights or same view with error message</returns>
        // 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(WalletAccessRightEditModel walletAccessRight)
        {
            if (!ModelState.IsValid)
            {
                this.AddError(SharedResource.ModelStateIsNotValid);
                walletAccessRight.Permissions = this.GetPermissions();
                return this.View(walletAccessRight);
            }

            var permission = this._commonService.ConvertPermissionStringToEnum(walletAccessRight.Permission);
            try
            {
                await
                    this._walletAccessRightService.EditWalletAccessRight(
                        walletAccessRight.Id,
                        permission,
                        walletAccessRight.AssignedUserId
                        );
            }
            catch (ServiceValidationException exception)
            {
                ModelState.AddModelErrors(exception);
                walletAccessRight.Permissions = this.GetPermissions();
                return this.View(walletAccessRight);
            }

            var userEmail = await this.GetEmailByUserId(walletAccessRight.AssignedUserId);
            this.AddSuccess(string.Format(WalletAccessRightResource.RightEdited, userEmail));
            return this.RedirectToAction(SharedConstant.Index);
        }

        /// <summary>
        ///     Address : GET: WalletAccessRights/Delete/5
        ///     return view with confirmation dialog for delete
        /// </summary>
        /// <param name="id">id of wallet access right</param>
        /// <returns>view</returns>
        public async Task<ActionResult> Delete(Guid id)
        {
            var walletAccessRight = await this._walletAccessRightService.GetWalletAccessRightById(id);

            if (walletAccessRight == null)
            {
                return this.HttpNotFound();
            }

            return this.View(Mapper.Map<WalletAccessRightModel>(walletAccessRight));
        }

        /// <summary>
        ///     Address : POST: WalletAccessRights/Delete/5
        ///     Delete wallet with given guid
        /// </summary>
        /// <param name="id">id of wallet access right</param>
        /// <returns>redirect to list with all wallet access rights</returns>
        [HttpPost, ActionName(SharedConstant.Delete)]
        [ValidateAntiForgeryToken]
        public async Task<RedirectToRouteResult> DeleteConfirmed(Guid id)
        {
            var walletAccessRightPermission =
                await this._walletAccessRightService.GetPermissionByWalletAccessRightId(id);
            // should not happen from front end - just some kind of attack can do this
            if (walletAccessRightPermission.Equals(PermissionEnum.Owner))
            {
                return this.RedirectToAction(SharedConstant.Index);
            }

            var walletAccessRight = await this._walletAccessRightService.GetWalletAccessRightById(id);
            var userEmail = await this.GetEmailByUserId(walletAccessRight.UserProfile.Guid);

            await this._walletAccessRightService.DeleteWalletAccessRight(id);

            this.AddSuccess(string.Format(WalletAccessRightResource.RightDeleted, userEmail));
            return this.RedirectToAction(SharedConstant.Index);
        }

        #region private

        private WalletAccessRightModel ConvertEntityToModelWithComboOptions(WalletAccessRight entity)
        {
            var model = Mapper.Map<WalletAccessRightModel>(entity);
            model.Permissions = this.GetPermissions();
            return model;
        }

        private WalletAccessRightEditModel ConvertEntityToEditModel(WalletAccessRight entity)
        {
            var model = Mapper.Map<WalletAccessRightEditModel>(entity);
            model.Permissions = this.GetPermissions();
            return model;
        }

        #endregion
    }
}