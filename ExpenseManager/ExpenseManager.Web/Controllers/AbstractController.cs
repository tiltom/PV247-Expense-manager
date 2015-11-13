using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Currencies;
using Microsoft.AspNet.Identity;
using ExpenseManager.Database.Contexts;

namespace ExpenseManager.Web.Controllers
{
    public abstract class AbstractController : Controller
    {
        /// <summary>
        ///     List of permission which can be used at front end
        /// </summary>
        protected static readonly List<PermissionEnum> AllowedPermissions = new List<PermissionEnum>
        {
            PermissionEnum.Read,
            PermissionEnum.Write
        };

        /// <summary>
        ///     Generated select options for front end editing of permissions
        /// </summary>
        protected static readonly List<SelectListItem> PermissionSelectList = AllowedPermissions
            .Select(permission => new SelectListItem {Value = permission.ToString(), Text = permission.ToString()})
            .ToList();

        private readonly UserContext db = new UserContext();

        /// <summary>
        ///     returns profile id of currenly logged user
        /// </summary>
        /// <returns> logged user profile id</returns>
        protected async Task<Guid> CurrentProfileId()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            return await this.db.Users.Where(u => u.Id == userId).Select(u => u.Profile.Guid).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Gets the default currency
        /// </summary>
        /// <returns>Default currency</returns>
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