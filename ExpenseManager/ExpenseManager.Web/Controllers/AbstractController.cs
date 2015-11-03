﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Users;
using ExpenseManager.Web.DatabaseContexts;
using Microsoft.AspNet.Identity;

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

        private readonly ApplicationDbContext db = new ApplicationDbContext();

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
        ///     Gets user profile of logged user
        /// </summary>
        /// <returns> return profile of logged user</returns>
        protected async Task<UserProfile> CurrentProfile()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            return await this.db.Users.Where(u => u.Id == userId).Select(u => u.Profile).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Gets id of Wallet for currently logged UserProfile
        /// </summary>
        /// <returns>WalletId</returns>
        protected async Task<Guid> GetUserWalletId()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            return
                await
                    this.db.Users.Where(u => u.Id == userId)
                        .Select(u => u.Profile.PersonalWallet.Guid)
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
                    this.db.Currencies.FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Gets the default category
        /// </summary>
        /// <returns>Default category</returns>
        protected async Task<Category> GetDefaultCategory()
        {
            return
                await
                    this.db.Categories.FirstOrDefaultAsync();
        }

        protected List<SelectListItem> GetPermissions()
        {
            return PermissionSelectList;
        }
    }
}