using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;
using ExpenseManager.Web.DatabaseContexts;
using ExpenseManager.Web.Models.WalletAcessRight;

namespace ExpenseManager.Web.Controllers
{
    public class WalletAcessRightController : AbstractController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// </summary>
        /// <returns></returns>
        // GET: WalletAcessRights
        public async Task<ActionResult> Index()
        {
            var id = await this.CurrentProfileId();
            var list = await this.db.WalletAccessRights.Where(right => right.Wallet.Owner.Guid == id).ToListAsync();
            return this.View(list.Select(ConvertEntityToModel));
        }

        /// <summary>
        ///     Address :  GET: WalletAcessRights/Create
        ///     get the view with combos for editing wallet access rights
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Create()
        {
            return
                this.View(await this.ConvertEntityToModelWithComboOptions(new WalletAccessRight
                {
                    Wallet = new Wallet
                    {
                        Guid = await this.GetUserWalletId()
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
        public async Task<ActionResult> Create(WalletAcessRightModel walletAccessRight)
        {
            if (ModelState.IsValid)
            {
                this.db.WalletAccessRights.Add(
                    await this.ConvertModelToEntity(walletAccessRight, new WalletAccessRight {Guid = Guid.NewGuid()}));
                await this.db.SaveChangesAsync();
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
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var walletAccessRight = await this.db.WalletAccessRights.FindAsync(id);
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
        public async Task<ActionResult> Edit(WalletAcessRightModel walletAccessRight)
        {
            var walletAccessRightEntity = await this.db.WalletAccessRights.FindAsync(walletAccessRight.Id);
            if (ModelState.IsValid)
            {
                await this.ConvertModelToEntity(walletAccessRight, walletAccessRightEntity);
                await this.db.SaveChangesAsync();
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
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var walletAccessRight = await this.db.WalletAccessRights.FindAsync(id);
            if (walletAccessRight == null)
            {
                return this.HttpNotFound();
            }
            return this.View(ConvertEntityToModel(walletAccessRight));
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
            var walletAccessRight = await this.db.WalletAccessRights.FindAsync(id);
            if (walletAccessRight.Permission.Equals(PermissionEnum.Owner))
            {
                return this.RedirectToAction("Index");
            }
            this.db.WalletAccessRights.Remove(walletAccessRight);
            await this.db.SaveChangesAsync();
            return this.RedirectToAction("Index");
        }

        #region protected

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region private

        private async Task<WalletAccessRight> ConvertModelToEntity(WalletAcessRightModel model, WalletAccessRight entity)
        {
            var defaultPermission = PermissionEnum.Read;
            Enum.TryParse(model.Permission, out defaultPermission);
            entity.Wallet = await this.db.Wallets.FindAsync(model.WalletId);
            entity.UserProfile = await this.db.UserProfiles.FirstOrDefaultAsync(u => u.Guid == model.AssignedUserId);
            entity.Permission = defaultPermission;
            return entity;
        }

        private static WalletAcessRightModel ConvertEntityToModel(WalletAccessRight entity)
        {
            return new WalletAcessRightModel
            {
                Id = entity.Guid,
                Permission = entity.Permission.ToString(),
                AssignedUserId = entity.UserProfile.Guid,
                AssignedUserName = entity.UserProfile.FirstName + " " + entity.UserProfile.LastName,
                WalletId = entity.Wallet.Guid
            };
        }

        private async Task<WalletAcessRightModel> ConvertEntityToModelWithComboOptions(WalletAccessRight entity)
        {
            var model = ConvertEntityToModel(entity);
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
            var currrentUserId = await this.CurrentProfileId();
            return
                await
                    this.db.UserProfiles
                        .Where(
                            u =>
                                u.WalletAccessRights.All(war => war.Wallet.Owner.Guid != currrentUserId) ||
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