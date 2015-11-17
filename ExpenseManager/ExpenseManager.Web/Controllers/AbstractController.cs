using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Currencies;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ExpenseManager.Web.Controllers
{
    public abstract class AbstractController : Controller
    {
        public const string DateFormat = "dd.MM.yyyy";
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

        private UserContext _context;

        protected UserContext UserContext
        {
            get { return this._context ?? (this._context = HttpContext.GetOwinContext().Get<UserContext>()); }
        }


        /// <summary>
        ///     returns profile id of currenly logged user
        /// </summary>
        /// <returns> logged user profile id</returns>
        protected async Task<Guid> CurrentProfileId()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            return
                await
                    UserContext.Users.Where(u => u.Id == userId).Select(u => u.Profile.Guid).FirstOrDefaultAsync();
        }


        /// <summary>
        ///     Gets the default currency
        /// </summary>
        /// <returns>Default currency</returns>
        protected async Task<Currency> GetDefaultCurrency()
        {
            return
                await
                    UserContext.Currencies.FirstOrDefaultAsync();
        }


        protected List<SelectListItem> GetPermissions()
        {
            return PermissionSelectList;
        }
    }
}