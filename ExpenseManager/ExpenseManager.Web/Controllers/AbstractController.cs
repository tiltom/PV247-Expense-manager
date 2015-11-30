using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExpenseManager.Database.Common;
using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ExpenseManager.Web.Controllers
{
    public abstract class AbstractController : Controller
    {
        public const string DateFormat = "dd.MM.yyyy";
        public const int PageSize = 5;


        #region protected

        /// <summary>
        ///     List of permission which can be used at front end
        /// </summary>
        protected static readonly Lazy<List<PermissionEnum>> AllowedPermissions = new Lazy<List<PermissionEnum>>(
            () => new List<PermissionEnum>
            {
                PermissionEnum.Read,
                PermissionEnum.Write
            });

        /// <summary>
        ///     Generated select options for front end editing of permissions
        /// </summary>
        protected static readonly Lazy<List<SelectListItem>> PermissionSelectList =
            new Lazy<List<SelectListItem>>(() =>
                AllowedPermissions.Value
                    .Select(
                        permission => new SelectListItem {Value = permission.ToString(), Text = permission.ToString()})
                    .ToList()
                );

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
            return PermissionSelectList.Value;
        }

        protected async Task<UserIdentity> CreateUser(string email, string firstName, string lastName)
        {
            var user = new UserIdentity
            {
                UserName = email,
                Email = email,
                CreationDate = DateTime.Now,
                Profile = new UserProfile
                {
                    PersonalWallet = new Wallet
                    {
                        Name = "Default Wallet",
                        Currency = await this.GetDefaultCurrency()
                    },
                    FirstName = firstName,
                    LastName = lastName
                }
            };

            user.Profile.WalletAccessRights = new List<WalletAccessRight>
            {
                new WalletAccessRight
                {
                    Permission = PermissionEnum.Owner,
                    UserProfile = user.Profile,
                    Wallet = user.Profile.PersonalWallet
                }
            };
            return user;
        }
    }

    #endregion
}