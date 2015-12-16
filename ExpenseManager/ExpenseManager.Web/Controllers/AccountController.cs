using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.HtmlHelpers;
using ExpenseManager.Database;
using ExpenseManager.Resources;
using ExpenseManager.Resources.AccountResources;
using ExpenseManager.Web.Constants;
using ExpenseManager.Web.Constants.LoginConstants;
using ExpenseManager.Web.Helpers;
using ExpenseManager.Web.Models.User;
using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace ExpenseManager.Web.Controllers
{
    [Authorize]
    public class AccountController : AbstractController
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
                this.AddError(SharedResource.ModelStateIsNotValid);
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
                    return this.View(LoginConstant.Lockout);
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", AccountResource.InvalidLoginAttempt);
                    return this.View(model);
            }
        }

        /// <summary>
        ///     Display registration form
        /// </summary>
        /// <returns>View</returns>
        [AllowAnonymous]
        public async Task<ActionResult> Register(string returnUrl)
        {
            string email = null;
            if (!string.IsNullOrEmpty(returnUrl))
            {
                var uri = returnUrl.Replace("%40", "@");
                var emailRegex = new Regex("userEmail=(?<email>.*@.*)&permission");
                email = emailRegex.Match(uri).Groups["email"].Value;
            }
            var model = new RegisterWithPasswordViewModel
            {
                Currencies = await this.GetCurrencies(),
                ReturnUrl = returnUrl,
                Email = email,
                IsExternal = !string.IsNullOrEmpty(email)
            };

            return this.View(model);
        }

        /// <summary>
        ///     Register UserProfile and create default wallet for him
        /// </summary>
        /// <param name="model">RegisterWithPasswordViewModel instance</param>
        /// <returns>View</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterWithPasswordViewModel model, string returnUrl)
        {
            this.IsCaptchaValid(SharedResource.CaptchaValidationFailed);
            if (!ModelState.IsValid)
            {
                this.AddError(SharedResource.ModelStateIsNotValid);
                model.Currencies = await this.GetCurrencies();
                return this.View(model);
            }

            var user = await this.CreateUser(model);
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(user, false, false);
                var userRole = await RoleManager.FindByNameAsync(UserIdentity.UserRole);
                await UserManager.AddToRoleAsync(user.Id, userRole.Name);

                if (string.IsNullOrWhiteSpace(returnUrl))
                    return this.RedirectToAction(SharedConstant.Index, SharedConstant.DashBoard);
                return this.RedirectToLocal(returnUrl);
            }
            this.AddErrors(result);
            model.Currencies = await this.GetCurrencies();

            // If we got this far, something failed, redisplay form
            return this.View(model);
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
                return this.View(SharedConstant.Error);
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return this.View(result.Succeeded ? LoginConstant.ConfirmEmail : SharedConstant.Error);
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
                Url.Action(LoginConstant.ExternalLoginCallback, LoginConstant.Account, new {ReturnUrl = returnUrl}));
        }

        /// <summary>
        ///     Login to application via external login service
        /// </summary>
        /// <param name="returnUrl">return URL after successful login</param>
        /// <returns>View</returns>
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return this.RedirectToAction(LoginConstant.Login);
            }

            // Sign in the UserProfile with this external login provider if the UserProfile already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, false);
            var name = loginInfo.ExternalIdentity.Name.Split(new[] {' '}, 2);
            var firstName = name[0];
            var lastName = name[1];
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return this.View(LoginConstant.Lockout);
                default:
                    // If the UserProfile does not have an account, then prompt the UserProfile to create an account
                    if (loginInfo.Login.LoginProvider == LoginConstant.Facebook)
                    {
                        var identity =
                            AuthenticationManager.GetExternalIdentity(DefaultAuthenticationTypes.ExternalCookie);
                        var accessToken = identity.FindFirstValue(LoginConstant.FacebookAccessToken);
                        var fb = new FacebookClient(accessToken);
                        dynamic myInfo = fb.Get("me?fields=first_name, email, last_name"); // specify the email field
                        loginInfo.Email = myInfo.email;
                        firstName = myInfo.first_name;
                        lastName = myInfo.last_name;
                    }
                    ViewBag.ReturnUrl = returnUrl;
                    this.AddSuccess(string.Format(AccountResource.SuccessfullyAuthenticated,
                        loginInfo.Login.LoginProvider));
                    return this.View(LoginConstant.ExternalLoginConfirmation,
                        new RegisterViewModel
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            Email = loginInfo.Email,
                            Currencies = await this.GetCurrencies()
                        });
            }
        }

        /// <summary>
        ///     Confirm login via external service as Google or FB
        /// </summary>
        /// <param name="model">RegisterViewModel instance</param>
        /// <param name="returnUrl">return URL after successful login</param>
        /// <returns>View</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(RegisterViewModel model,
            string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction(SharedConstant.Index, LoginConstant.Manage);
            }

            if (ModelState.IsValid)
            {
                // Get the information about the UserProfile from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return this.View(LoginConstant.ExternalLoginFailure);
                }

                var user = await this.CreateUser(model);

                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    var userRole = await RoleManager.FindByNameAsync(UserIdentity.UserRole);
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
            model.Currencies = await this.GetCurrencies();
            ViewBag.ReturnUrl = returnUrl;
            return this.View(model);
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
            return this.RedirectToAction(SharedConstant.Index, SharedConstant.DashBoard);
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
            return this.RedirectToAction(SharedConstant.Index, SharedConstant.DashBoard);
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
                    properties.Dictionary[LoginConstant.XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}