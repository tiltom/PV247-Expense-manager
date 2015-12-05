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
using ExpenseManager.Web.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ExpenseManager.Web.Controllers
{
    public abstract class AbstractController : Controller
    {
        public const string DateFormat = "dd.MM.yyyy";
        public const string Error = "Error";
        public const string Success = "Success";
        public const int PageSize = 5;

        protected void AddSuccess(string message)
        {
            TempData[Success] = message;
        }

        protected void AddError(string message)
        {
            TempData[Error] = message;
        }

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

        /// <summary>
        ///     Returns user profile by it's Email
        /// </summary>
        /// <param name="email">User mail</param>
        /// <returns>Desired user profile</returns>
        protected async Task<Guid> GetUserProfileByEmail(string email)
        {
            return
                await UserContext.Users.Where(x => x.Email == email).Select(u => u.Profile.Guid).FirstOrDefaultAsync();
        }


        protected List<SelectListItem> GetPermissions()
        {
            return PermissionSelectList.Value;
        }

        protected async Task<List<SelectListItem>> GetCurrencies()
        {
            var currencies = await UserContext.Currencies.Select(currency => new SelectListItem
            {
                Text = currency.Name,
                Value = currency.Guid.ToString()
            }).ToListAsync();
            return currencies;
        }

        protected async Task<UserIdentity> CreateUser(RegisterViewModel model)
        {
            var currency = await UserContext.Currencies.FirstOrDefaultAsync(x => x.Guid == model.CurrencyId);

            var user = new UserIdentity
            {
                UserName = model.Email,
                Email = model.Email,
                CreationDate = DateTime.Now,
                Profile = new UserProfile
                {
                    PersonalWallet = new Wallet
                    {
                        Name = "Default Wallet",
                        Currency = currency
                    },
                    FirstName = model.FirstName,
                    LastName = model.LastName
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