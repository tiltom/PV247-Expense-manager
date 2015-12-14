using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ExpenseManager.Resources.UsersAdminResources;
using ExpenseManager.Web.Constants.UserConstants;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace ExpenseManager.Web.Models.User
{
    /// <summary>
    ///     Model for index view of user management
    /// </summary>
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            Logins = Enumerable.Empty<UserLoginInfo>();
        }

        /// <summary>
        ///     If user already has password
        /// </summary>
        public bool HasPassword { get; set; }

        /// <summary>
        ///     List of user logins (also includes external logins via Google or Facebook)
        /// </summary>
        public IEnumerable<UserLoginInfo> Logins { get; set; }

        /// <summary>
        ///     Phone number of user
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     Is two factor autentization enabled
        /// </summary>
        public bool TwoFactor { get; set; }

        /// <summary>
        ///     Is remember me option enabled
        /// </summary>
        public bool BrowserRemembered { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public ManageLoginsViewModel()
        {
            CurrentLogins = Enumerable.Empty<UserLoginInfo>();
            OtherLogins = Enumerable.Empty<AuthenticationDescription>();
        }

        /// <summary>
        ///     containt login information about currently logged user
        /// </summary>
        public IEnumerable<UserLoginInfo> CurrentLogins { get; set; }

        /// <summary>
        ///     list with options of other login options available to user
        /// </summary>
        public IEnumerable<AuthenticationDescription> OtherLogins { get; set; }
    }

    /// <summary>
    ///     Model for creating new password
    /// </summary>
    public class SetPasswordViewModel
    {
        /// <summary>
        ///     New password
        /// </summary>
        [Required]
        [StringLength(UserConstant.NameMaximumLength, ErrorMessageResourceType = typeof (UsersAdminResource),
            ErrorMessageResourceName = "MinimumLengthErrorMessage", MinimumLength = UserConstant.PasswordMinimumLength)]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword", ResourceType = typeof (UsersAdminResource))]
        public string NewPassword { get; set; }

        /// <summary>
        ///     New password confirmation
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmNewPassword", ResourceType = typeof (UsersAdminResource))]
        [Compare("Password",
            ErrorMessageResourceType = typeof (UsersAdminResource),
            ErrorMessageResourceName = "PasswordsDoNotMatch")]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    ///     Model for changing existing password
    /// </summary>
    public class ChangePasswordViewModel
    {
        /// <summary>
        ///     Old password of user
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "CurrentPassword", ResourceType = typeof (UsersAdminResource))]
        public string OldPassword { get; set; }

        /// <summary>
        ///     New password
        /// </summary>
        [Required]
        [StringLength(UserConstant.NameMaximumLength, ErrorMessageResourceType = typeof (UsersAdminResource),
            ErrorMessageResourceName = "MinimumLengthErrorMessage", MinimumLength = UserConstant.PasswordMinimumLength)]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword", ResourceType = typeof (UsersAdminResource))]
        public string NewPassword { get; set; }

        /// <summary>
        ///     Confirmation of new password
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmNewPassword", ResourceType = typeof (UsersAdminResource))]
        [Compare("NewPassword",
            ErrorMessageResourceType = typeof (UsersAdminResource),
            ErrorMessageResourceName = "PasswordsDoNotMatch")]
        public string ConfirmPassword { get; set; }
    }
}