using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;
using ExpenseManager.Web.Common;
using ExpenseManager.Web.DatabaseContexts;
using ExpenseManager.Web.Models.User;
using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace ExpenseManager.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationRoleManager _roleManager;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager,
            ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
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

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return this._roleManager ??
                       (this._roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>());
            }
            private set { this._roleManager = value; }
        }

        /// <summary>
        ///     Display login page
        /// </summary>
        /// <param name="returnUrl">Return url used by external logins</param>
        /// <returns>View</returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return this.View();
        }

        /// <summary>
        ///     Log in UserProfile to application
        /// </summary>
        /// <param name="model">LoginViewModel instance</param>
        /// <param name="returnUrl">Return url used to redirection after successful login</param>
        /// <returns>View</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return this.View(model);
            }
        }

        /// <summary>
        ///     Display registration form
        /// </summary>
        /// <returns>View</returns>
        [AllowAnonymous]
        public ActionResult Register()
        {
            return this.View();
        }

        /// <summary>
        ///     Register UserProfile and create default wallet for him
        /// </summary>
        /// <param name="model">RegisterViewModel instance</param>
        /// <returns>View</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return this.View(model);

            var user = this.CreateUser(model.Email, model.FirstName, model.LastName);
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(user, false, false);
                var userRole = await RoleManager.FindByNameAsync("UserProfile");
                await UserManager.AddToRoleAsync(user.Id, userRole.Name);

                return this.RedirectToAction("Index", "Home");
            }
            this.AddErrors(result);

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        private Currency GetDefaultCurrency()
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var currency = context.Currencies.FirstOrDefault(c => c.Symbol == "Kč");
            return currency;
        }


        /// <summary>
        ///     Confirm UserProfile email
        /// </summary>
        /// <param name="userId">id of UserProfile</param>
        /// <param name="code">verification code</param>
        /// <returns>View</returns>
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return this.View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return this.View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        /// <summary>
        ///     Login from external service like FB or Google
        /// </summary>
        /// <param name="provider">external login provider</param>
        /// <param name="returnUrl">return URL after successful login</param>
        /// <returns>View</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider,
                Url.Action("ExternalLoginCallback", "Account", new {ReturnUrl = returnUrl}));
        }

        /// <summary>
        ///     Login to application via external login service
        /// </summary>
        /// <param name="returnUrl">return URL after sucessful login</param>
        /// <returns>View</returns>
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return this.RedirectToAction("Login");
            }

            // Sign in the UserProfile with this external login provider if the UserProfile already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, false);
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                case SignInStatus.Failure:
                default:
                    // If the UserProfile does not have an account, then prompt the UserProfile to create an account
                    if (loginInfo.Login.LoginProvider == "Facebook")
                    {
                        var identity =
                            AuthenticationManager.GetExternalIdentity(DefaultAuthenticationTypes.ExternalCookie);
                        var access_token = identity.FindFirstValue("FacebookAccessToken");
                        var fb = new FacebookClient(access_token);
                        dynamic myInfo = fb.Get("/me?fields=email"); // specify the email field
                        loginInfo.Email = myInfo.email;
                    }
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return this.View("ExternalLoginConfirmation",
                        new ExternalLoginConfirmationViewModel {Email = loginInfo.Email});
            }
        }

        /// <summary>
        ///     Confirm login via external service as Google or FB
        /// </summary>
        /// <param name="model">ExternalLoginConfirmationViewModel instance</param>
        /// <param name="returnUrl">return URL after sucessful login</param>
        /// <returns>View</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model,
            string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the UserProfile from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return this.View("ExternalLoginFailure");
                }

                var user = this.CreateUser(model.Email, "external", "registration"); // TODO: add first and last name

                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    var userRole = await RoleManager.FindByNameAsync("UserProfile");
                    await UserManager.AddToRoleAsync(user.Id, userRole.Name);

                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, false, false);
                        return this.RedirectToLocal(returnUrl);
                    }
                }
                this.AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return this.View(model);
        }

        private UserIdentity CreateUser(string email, string firstName, string lastName)
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
                        Currency = this.GetDefaultCurrency()
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

        /// <summary>
        ///     Log off from application
        /// </summary>
        /// <returns>Main page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return this.RedirectToAction("Index", "Home");
        }

        /// <summary>
        ///     Display failure login view after failed attempt of login via external service
        /// </summary>
        /// <returns>View</returns>
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return this.View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._userManager != null)
                {
                    this._userManager.Dispose();
                    this._userManager = null;
                }

                if (this._signInManager != null)
                {
                    this._signInManager.Dispose();
                    this._signInManager = null;
                }

                if (this._roleManager != null)
                {
                    this._roleManager.Dispose();
                    this._roleManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }
            return this.RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties {RedirectUri = RedirectUri};
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}