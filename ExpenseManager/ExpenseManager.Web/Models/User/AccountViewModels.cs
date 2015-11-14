using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ExpenseManager.Web.Models.User
{
    /// <summary>
    ///     Model for external login to application
    /// </summary>
    public class ExternalLoginConfirmationViewModel
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
        public string FirstName { get; set; }

        /// <summary>
        ///     Last name of user
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }
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
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    /// <summary>
    ///     Model for registration of user
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        ///     Email of user
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        ///     Password of user
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        ///     Password confirmation
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password",
            ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        ///     First name of user
        /// </summary>
        [Required]
        [Display(Name = "First name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; }

        /// <summary>
        ///     Last name of user
        /// </summary>
        [Required]
        [Display(Name = "Last name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string LastName { get; set; }

        /// <summary>
        ///     List of all roles supported in system
        /// </summary>
        public List<SelectListItem> RolesList { get; set; }

        /// <summary>
        ///     List of selected roles for user
        /// </summary>
        public IEnumerable<string> SelectedRoles { get; set; }
    }
}