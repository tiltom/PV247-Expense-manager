using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExpenseManager.Web.Models.User;
using ExpenseManager.Web.Models.Wallet;
using Microsoft.AspNet.Identity;

namespace ExpenseManager.Web.Controllers
{
    [Authorize]
    public class WalletController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();


        // GET: Wallets/Edit
        public async Task<ActionResult> Edit()
        {
            var id = HttpContext.User.Identity.GetUserId();
            var wallet = await this.db.Wallets.Where(u => u.Owner.Id == id).FirstOrDefaultAsync();
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

        private async Task<List<SelectListItem>> GetCurrencies()
        {
            var currencies = await this.db.Currencies.Select(currency => new SelectListItem
            {
                Text = currency.Name,
                Value = currency.Guid.ToString()
            }).ToListAsync();
            return currencies;
        }

        // POST: Wallets/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(WalletEditModel wallet)
        {
            if (ModelState.IsValid)
            {
                var walletEntity = this.db.Wallets.Find(wallet.Guid);
                walletEntity.Currency = this.db.Currencies.Find(wallet.CurrencyId);
                walletEntity.Name = wallet.Name;
                await this.db.SaveChangesAsync();
                return this.RedirectToAction("Index", "Manage");
            }
            return this.View(wallet);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}