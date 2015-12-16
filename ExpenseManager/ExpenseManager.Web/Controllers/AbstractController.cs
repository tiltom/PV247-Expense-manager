using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExpenseManager.Database;
using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;
using ExpenseManager.Resources;
using ExpenseManager.Web.Constants;
using ExpenseManager.Web.Constants.UserConstants;
using ExpenseManager.Web.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using QueryableExtensions = System.Data.Entity.QueryableExtensions;

namespace ExpenseManager.Web.Controllers
{
    public abstract class AbstractController : Controller
    {
        protected void AddSuccess(string message)
        {
            TempData[SharedConstant.Success] = message;
        }

        protected void AddError(string message)
        {
            TempData[SharedConstant.Error] = message;
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
        ///     Returns profile id of currenly logged user
        /// </summary>
        /// <returns> Logged user profile id</returns>
        protected async Task<Guid> CurrentProfileId()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            return
                await
                    UserContext.Users.Where(user => user.Id == userId)
                        .Select(user => user.Profile.Guid)
                        .FirstOrDefaultAsync();
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
                await
                    UserContext.Users.Where(user => user.Email == email)
                        .Select(user => user.Profile.Guid)
                        .FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Returns user email that belong to the specified user
        /// </summary>
        /// <param name="userId">Id of a user</param>
        /// <returns>Desired email</returns>
        protected async Task<string> GetEmailByUserId(Guid userId)
        {
            return
                await
                    UserContext.Users.Where(user => user.Profile.Guid == userId)
                        .Select(user => user.Email)
                        .FirstOrDefaultAsync();
        }

        protected List<SelectListItem> GetPermissions()
        {
            return PermissionSelectList.Value;
        }

        protected async Task<List<SelectListItem>> GetCurrencies()
        {
            var currencies =
                await QueryableExtensions.ToListAsync(UserContext.Currencies.Select(currency => new SelectListItem
                {
                    Text = currency.Name,
                    Value = currency.Guid.ToString()
                }));
            return currencies;
        }

        protected async Task<UserIdentity> CreateUser(RegisterViewModel model)
        {
            var currency =
                await UserContext.Currencies.FirstOrDefaultAsync(userCurrency => userCurrency.Guid == model.CurrencyId);

            var user = new UserIdentity
            {
                UserName = model.Email,
                Email = model.Email,
                CreationDate = DateTime.Now,
                Profile = new UserProfile
                {
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
                    Wallet = new Wallet
                    {
                        Name =
                            UserConstant.FormatWalletName(
                                model.FirstName,
                                model.LastName,
                                SharedResource.DefaultWallet),
                        Currency = currency
                    }
                }
            };
            return user;
        }
    }

    #endregion
}