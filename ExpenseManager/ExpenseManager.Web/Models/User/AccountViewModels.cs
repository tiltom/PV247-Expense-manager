using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using ExpenseManager.Resources;
using ExpenseManager.Resources.UsersAdminResources;
using ExpenseManager.Web.Constants.UserConstants;

namespace ExpenseManager.Web.Models.User
{
    /// <summary>
    ///     Model for external login to application
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        ///     Email address of user
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        ///     First name of user
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "FirstName", ResourceType = typeof (UsersAdminResource))]
        [StringLength(UserConstant.NameMaximumLength, ErrorMessageResourceType = typeof (UsersAdminResource),
            ErrorMessageResourceName = "MinimumLengthErrorMessage", MinimumLength = UserConstant.NameMinimumLength)]
        public string FirstName { get; set; }

        /// <summary>
        ///     Last name of user
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "LastName", ResourceType = typeof (UsersAdminResource))]
        [StringLength(UserConstant.NameMaximumLength, ErrorMessageResourceType = typeof (UsersAdminResource),
            ErrorMessageResourceName = "MinimumLengthErrorMessage", MinimumLength = UserConstant.NameMinimumLength)]
        public string LastName { get; set; }

        /// <summary>
        ///     Currency of the wallet
        /// </summary>
        [Display(Name = "Currency", ResourceType = typeof (SharedResource))]
        [Required]
        public Guid CurrencyId { get; set; }

        /// <summary>
        ///     List of currencies available for the user
        /// </summary>
        [Display(Name = "WalletCurrency", ResourceType = typeof (UsersAdminResource))]
        public List<SelectListItem> Currencies { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    /// <summary>
    ///     Model for login page view
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        ///     Email address of user
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        ///     User's password
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        ///     Remember me option - if user name and password should be remembered
        /// </summary>
        [Display(Name = "RememberMe", ResourceType = typeof (UsersAdminResource))]
        public bool RememberMe { get; set; }
    }

    /// <summary>
    ///     Model for registration of user
    /// </summary>
    public class RegisterWithPasswordViewModel : RegisterViewModel
    {
        public RegisterWithPasswordViewModel()
        {
            SelectedRoles = Enumerable.Empty<string>();
        }
        /// <summary>
        ///     Password of user
        /// </summary>
        [Required]
        [StringLength(UserConstant.NameMaximumLength, ErrorMessageResourceType = typeof (UsersAdminResource),
            ErrorMessageResourceName = "MinimumLengthErrorMessage", MinimumLength = UserConstant.PasswordMinimumLength)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        ///     Password confirmation
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof (UsersAdminResource))]
        [System.ComponentModel.DataAnnotations.Compare("Password",
            ErrorMessageResourceType = typeof (UsersAdminResource),
            ErrorMessageResourceName = "PasswordsDoNotMatch")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        ///     List of all roles supported in system
        /// </summary>
        public List<SelectListItem> RolesList { get; set; }

        /// <summary>
        ///     List of selected roles for user
        /// </summary>
        public IEnumerable<string> SelectedRoles { get; set; }

        /// <summary>
        /// Url to go to when leaving form.
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}