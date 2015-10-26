using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Users;
using ExpenseManager.Web.DatabaseContexts;
using Microsoft.AspNet.Identity;

namespace ExpenseManager.Web.Controllers
{
    public class AbstractController : Controller
    {
        protected static readonly List<PermissionEnum> AllowedPermissions = new List<PermissionEnum>
        {
            PermissionEnum.Read,
            PermissionEnum.Write
        };

        protected static readonly List<SelectListItem> PermissionSelectList = AllowedPermissions
            .Select(permission => new SelectListItem {Value = permission.ToString(), Text = permission.ToString()})
            .ToList();

        private readonly ApplicationDbContext db = new ApplicationDbContext();

        protected async Task<Guid> CurrentProfileId()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            return await this.db.Users.Where(u => u.Id == userId).Select(u => u.Profile.Guid).FirstOrDefaultAsync();
        }

        protected async Task<UserProfile> CurrentProfile()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            return await this.db.Users.Where(u => u.Id == userId).Select(u => u.Profile).FirstOrDefaultAsync();
        }

        /// </summary>
        /// Gets id of Wallet for currently logged UserProfile
        /// <summary>
        ///     <returns>WalletId</returns>
        protected async Task<Guid> GetUserWalletId()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            return
                await
                    this.db.Users.Where(u => u.Id == userId)
                        .Select(u => u.Profile.PersonalWallet.Guid)
                        .FirstOrDefaultAsync();
        }

        protected async Task<Currency> GetDefaultCurrency()
        {
            return
                await
                    this.db.Currencies.FirstOrDefaultAsync();
        }

        protected List<SelectListItem> GetPermissions()
        {
            return PermissionSelectList;
        }
    }
}