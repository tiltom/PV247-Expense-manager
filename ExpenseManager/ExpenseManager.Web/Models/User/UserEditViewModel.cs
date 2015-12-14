using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using ExpenseManager.Resources.UsersAdminResources;

namespace ExpenseManager.Web.Models.User
{
    /// <summary>
    ///     Model for editing user
    /// </summary>
    public class UserEditViewModel
    {
        public UserEditViewModel()
        {
            RolesList = Enumerable.Empty<SelectListItem>();
            SelectedRoles = Enumerable.Empty<string>();
        }

        /// <summary>
        ///     Id of user, is hidden on all pages
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Email address of user
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        public string Email { get; set; }


        /// <summary>
        ///     First name of user
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "FirstName", ResourceType = typeof (UsersAdminResource))]
        public string FirstName { get; set; }

        /// <summary>
        ///     Last name of user
        /// </summary>
        [Display(Name = "LastName", ResourceType = typeof (UsersAdminResource))]
        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }

        /// <summary>
        ///     All supported roles in system
        /// </summary>
        public IEnumerable<SelectListItem> RolesList { get; set; }

        /// <summary>
        ///     Selected roles for user
        /// </summary>
        [Display(Name = "SelectedRoles", ResourceType = typeof (UsersAdminResource))]
        public IEnumerable<string> SelectedRoles { get; set; }
    }
}