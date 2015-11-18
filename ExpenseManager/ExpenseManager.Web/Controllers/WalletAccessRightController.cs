using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;
using ExpenseManager.Web.Models.WalletAccessRight;

namespace ExpenseManager.Web.Controllers
{
    public class WalletAccessRightController : AbstractController
    {
        private readonly IWalletsProvider _db = ProvidersFactory.GetNewWalletsProviders();

        /// <summary>
        /// </summary>
        /// <returns></returns>
        // GET: WalletAccessRights
        public async Task<ActionResult> Index()
        {
            var id = await this.CurrentProfileId();
            var accessRightModels =
                await
                    this._db.WalletAccessRights.Where(right => right.Wallet.Owner.Guid == id)
                        .ProjectTo<WalletAccessRightModel>()
                        .ToListAsync();
            return this.View(accessRightModels);
        }

        /// <summary>
        ///     Address :  GET: WalletAcessRights/Create
        ///     get the view with combos for editing wallet access rights
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Create()
        {
            var profileId = await this.CurrentProfileId();
            var walletId = await this._db.Wallets
                .Where(w => w.Owner.Guid == profileId)
                .Select(w => w.Guid)
                .FirstOrDefaultAsync();
            return
                this.View(await this.ConvertEntityToModelWithComboOptions(new WalletAccessRight
                {
                    Wallet = new Wallet
                    {
                        Guid = walletId
                    },
                    UserProfile = new UserProfile()
                }));
        }

        /// <summary>
        ///     Address : POST: WalletAcessRights/Create
        ///     creates wallet access rights for given parameters
        /// </summary>
        /// <returns>redirect to list with all access rights or same view with error message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(WalletAccessRightModel walletAccessRight)
        {
            if (ModelState.IsValid)
            {
                await this._db.AddOrUpdateAsync(
                    await this.ConvertModelToEntity(walletAccessRight, new WalletAccessRight {Guid = Guid.NewGuid()}));
                return this.RedirectToAction("Index");
            }
            walletAccessRight.Users = await this.GetUsers(null);
            walletAccessRight.Permissions = this.GetPermissions();
            return this.View(walletAccessRight);
        }


        /// <summary>
        ///     Address : GET: WalletAcessRights/Edit/5
        ///     get the view with combos for editing wallet access rights
        /// </summary>
        /// <param name="id">id of wallet access right</param>
        /// <returns>view</returns>
        public async Task<ActionResult> Edit(Guid id)
        {
            var walletAccessRight =
                await this._db.WalletAccessRights.Where(war => war.Guid.Equals(id)).FirstOrDefaultAsync();
            if (walletAccessRight == null)
            {
                return this.HttpNotFound();
            }
            return this.View(await this.ConvertEntityToModelWithComboOptions(walletAccessRight));
        }

        /// <summary>
        ///     Address: POST: WalletAcessRights/Edit/5
        ///     saves the changes in wallet access rights
        /// </summary>
        /// <param name="walletAccessRight"> model of access rights for front end</param>
        /// <returns>redirect to list with all access rights or same view with error message</returns>
        // 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(WalletAccessRightModel walletAccessRight)
        {
            var walletAccessRightEntity =
                await
                    this._db.WalletAccessRights.Where(war => war.Guid.Equals(walletAccessRight.Id))
                        .FirstOrDefaultAsync();
            if (ModelState.IsValid)
            {
                await this.ConvertModelToEntity(walletAccessRight, walletAccessRightEntity);
                return this.RedirectToAction("Index");
            }
            walletAccessRight.Users = await this.GetUsers(walletAccessRightEntity.UserProfile.Guid);
            return this.View(walletAccessRight);
        }

        /// <summary>
        ///     Address : GET: WalletAcessRights/Delete/5
        ///     return view with confirmation dialog for delete
        /// </summary>
        /// <param name="id">id of wallet access right</param>
        /// <returns>view</returns>
        public async Task<ActionResult> Delete(Guid id)
        {
            var walletAccessRight =
                await this._db.WalletAccessRights.Where(war => war.Guid.Equals(id)).FirstOrDefaultAsync();
            if (walletAccessRight == null)
            {
                return this.HttpNotFound();
            }
            return this.View(Mapper.Map<WalletAccessRightModel>(walletAccessRight));
        }

        /// <summary>
        ///     Address : POST: WalletAcessRights/Delete/5
        ///     Delete wallet with given guid
        /// </summary>
        /// <param name="id">id of wallet access right</param>
        /// <returns>redirect to list with all wallet access rights</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<RedirectToRouteResult> DeleteConfirmed(Guid id)
        {
            var walletAccessRight =
                await this._db.WalletAccessRights.Where(war => war.Guid.Equals(id)).FirstOrDefaultAsync();
            if (walletAccessRight.Permission.Equals(PermissionEnum.Owner))
            {
                return this.RedirectToAction("Index");
            }

            await this._db.DeteleAsync(walletAccessRight);
            return this.RedirectToAction("Index");
        }

        #region private

        private async Task<WalletAccessRight> ConvertModelToEntity(WalletAccessRightModel model,
            WalletAccessRight entity)
        {
            var defaultPermission = PermissionEnum.Read;
            Enum.TryParse(model.Permission, out defaultPermission);

            entity.Wallet = await this._db.Wallets.Where(w => w.Guid == model.WalletId).FirstOrDefaultAsync();
            entity.UserProfile = await this._db.UserProfiles.FirstOrDefaultAsync(u => u.Guid == model.AssignedUserId);
            entity.Permission = defaultPermission;
            return entity;
        }

        private async Task<WalletAccessRightModel> ConvertEntityToModelWithComboOptions(WalletAccessRight entity)
        {
            var model = Mapper.Map<WalletAccessRightModel>(entity);
            model.Users = await this.GetUsers(entity.UserProfile.Guid);
            model.Permissions = this.GetPermissions();
            return model;
        }

        /// <summary>
        ///     get users id from context
        /// </summary>
        /// <returns>string with id</returns>
        /// <summary>
        ///     get users without rights created to UserProfile wallet
        /// </summary>
        /// <returns>list of users</returns>
        private async Task<List<SelectListItem>> GetUsers(Guid? userId)
        {
            var currentUserId = await this.CurrentProfileId();
            return
                await
                    this._db.UserProfiles
                        .Where(
                            u =>
                                u.WalletAccessRights.All(war => war.Wallet.Owner.Guid != currentUserId) ||
                                u.Guid == userId)
                        .Select(
                            user =>
                                new SelectListItem
                                {
                                    Value = user.Guid.ToString(),
                                    Text = user.FirstName + " " + user.LastName
                                })
                        .ToListAsync();
        }

        #endregion
    }
}