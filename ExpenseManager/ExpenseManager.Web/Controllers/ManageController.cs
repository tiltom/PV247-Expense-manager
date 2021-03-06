﻿using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExpenseManager.Resources.ManageResources;
using ExpenseManager.Web.Constants;
using ExpenseManager.Web.Constants.LoginConstants;
using ExpenseManager.Web.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace ExpenseManager.Web.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return this._signInManager ??
                       (this._signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>());
            }
            private set { this._signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return this._userManager ??
                       (this._userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());
            }
            private set { this._userManager = value; }
        }

        /// <summary>
        ///     Display UserProfile management view
        /// </summary>
        /// <param name="message">message about changes in account settings</param>
        /// <returns></returns>
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess
                    ? ManageResource.PasswordChanged
                    : message == ManageMessageId.SetPasswordSuccess
                        ? ManageResource.PasswordSet
                        : message == ManageMessageId.SetTwoFactorSuccess
                            ? ManageResource.TwoFactorAuthProviderSet
                            : message == ManageMessageId.Error
                                ? ManageResource.ErrorOccured
                                : message == ManageMessageId.AddPhoneSuccess
                                    ? ManageResource.PhoneNumberAdded
                                    : message == ManageMessageId.RemovePhoneSuccess
                                        ? ManageResource.PhoneNumberRemoved
                                        : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = this.HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return this.View(model);
        }

        /// <summary>
        ///     Remove used external login
        /// </summary>
        /// <param name="loginProvider">provider for external login</param>
        /// <param name="providerKey">provider key</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result =
                await
                    UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                        new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, false, false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return this.RedirectToAction(LoginConstant.ManageLogins, new {Message = message});
        }

        /// <summary>
        ///     Display change password form
        /// </summary>
        /// <returns>View</returns>
        public ActionResult ChangePassword()
        {
            return this.View();
        }

        /// <summary>
        ///     Change UserProfile password
        /// </summary>
        /// <param name="model">ChangePasswordViewModel model</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }
            var result =
                await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, false, false);
                }
                return this.RedirectToAction(SharedConstant.Index, new {Message = ManageMessageId.ChangePasswordSuccess});
            }
            this.AddErrors(result);
            return this.View(model);
        }

        /// <summary>
        ///     Display set UserProfile password form
        /// </summary>
        /// <returns>View</returns>
        public ActionResult SetPassword()
        {
            return this.View();
        }

        /// <summary>
        ///     Set new password for logged in UserProfile
        /// </summary>
        /// <param name="model">SetPasswordViewModel</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, false, false);
                    }
                    return this.RedirectToAction(SharedConstant.Index,
                        new {Message = ManageMessageId.SetPasswordSuccess});
                }
                this.AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        /// <summary>
        ///     Display manage external logins form
        /// </summary>
        /// <param name="message">message which is displayed when external login is added or removed</param>
        /// <returns>View</returns>
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess
                    ? ManageResource.ExternalLoginRemoved
                    : message == ManageMessageId.Error
                        ? ManageResource.ErrorOccured
                        : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return this.View(SharedConstant.Error);
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins =
                AuthenticationManager.GetExternalAuthenticationTypes()
                    .Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider))
                    .ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return this.View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        /// <summary>
        ///     Link external login with current UserProfile account
        /// </summary>
        /// <param name="provider">external login provider</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current UserProfile
            return new AccountController.ChallengeResult(provider,
                Url.Action(LoginConstant.LinkLoginCallback, LoginConstant.Manage),
                User.Identity.GetUserId());
        }

        /// <summary>
        ///     Display manage external logins
        /// </summary>
        /// <returns>View</returns>
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo =
                await AuthenticationManager.GetExternalLoginInfoAsync(LoginConstant.XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return this.RedirectToAction(LoginConstant.ManageLogins, new {Message = ManageMessageId.Error});
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded
                ? this.RedirectToAction(LoginConstant.ManageLogins)
                : this.RedirectToAction(LoginConstant.ManageLogins, new {Message = ManageMessageId.Error});
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this._userManager != null)
            {
                this._userManager.Dispose();
                this._userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}