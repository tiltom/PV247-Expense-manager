using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExpenseManager.Entity.Wallets;
using ExpenseManager.Web.Models.User;
using ExpenseManager.Web.Models.WalletAcessRight;
using Microsoft.AspNet.Identity;

namespace ExpenseManager.Web.Controllers
{
    public class WalletAcessRightsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: WalletAcessRights
        public async Task<ActionResult> Index()
        {
            var id = HttpContext.User.Identity.GetUserId();
            var list = await this.db.WalletAccessRights.Where(right => right.Wallet.Owner.Id == id).ToListAsync();
            return this.View(list.Select(this.ConvertEntityToModel));
        }

        // GET: WalletAcessRights/Create
        public async Task<ActionResult> Create()
        {
            return
                this.View(new WalletAcessRightModel
                {
                    WalletId = await this.GetUserWalletId(),
                    Users = await this.GetUsers()
                });
        }

        // POST: WalletAcessRights/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            walletAccessRight.Users = await this.GetUsers();
            return this.View(walletAccessRight);
        }

        // GET: WalletAcessRights/Edit/5
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

        // POST: WalletAcessRights/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(WalletAcessRightModel walletAccessRight)
        {
            if (ModelState.IsValid)
            {
                var walletAccessRightEntity = await this.db.WalletAccessRights.FindAsync(walletAccessRight.Id);
                await this.ConvertModelToEntity(walletAccessRight, walletAccessRightEntity);
                await this.db.SaveChangesAsync();
                return this.RedirectToAction("Index");
            }
            walletAccessRight.Users = await this.GetUsers();
            return this.View(walletAccessRight);
        }

        // GET: WalletAcessRights/Delete/5
        // TODO: cannot remove owner acess right !
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
            return this.View(walletAccessRight);
        }

        // POST: WalletAcessRights/Delete/5
        // TODO: cannot remove owner acess right !
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var walletAccessRight = await this.db.WalletAccessRights.FindAsync(id);
            this.db.WalletAccessRights.Remove(walletAccessRight);
            await this.db.SaveChangesAsync();
            return this.RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }
            base.Dispose(disposing);
        }

        private async Task<WalletAccessRight> ConvertModelToEntity(WalletAcessRightModel model, WalletAccessRight entity)
        {
            entity.Wallet = await this.db.Wallets.FindAsync(model.WalletId);
            entity.User = await this.db.Users.FirstOrDefaultAsync(u => u.Id == model.AssignedUserId);
            entity.Permission = model.Permission;
            return entity;
        }

        private WalletAcessRightModel ConvertEntityToModel(WalletAccessRight entity)
        {
            return new WalletAcessRightModel
            {
                Id = entity.Guid,
                Permission = entity.Permission,
                AssignedUserId = entity.User.Id,
                AssignedUserName = entity.User.UserName,
                WalletId = entity.Wallet.Guid
            };
        }

        private async Task<WalletAcessRightModel> ConvertEntityToModelWithComboOptions(WalletAccessRight entity)
        {
            var model = this.ConvertEntityToModel(entity);
            model.Users = await this.GetUsers();
            return model;
        }

        private async Task<Guid> GetUserWalletId()
        {
            var id = HttpContext.User.Identity.GetUserId();
            var walletEntity = await this.db.Wallets.Where(wallet => wallet.Owner.Id == id).FirstOrDefaultAsync();
            return walletEntity.Guid;
        }

        private async Task<List<SelectListItem>> GetUsers()
        {
            return
                await this.db.Users.Select(user => new SelectListItem {Value = user.Id, Text = user.UserName})
                    .ToListAsync();
        }
    }
}