using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ExpenseManager.Web.Models.User
{
    /// <summary>
    ///     Model for editing user
    /// </summary>
    public class UserEditViewModel
    {
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
        [DisplayName("First name")]
        public string FirstName { get; set; }

        /// <summary>
        ///     Last name of user
        /// </summary>
        [DisplayName("Last name")]
        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }

        /// <summary>
        ///     All supported roles in system
        /// </summary>
        public IEnumerable<SelectListItem> RolesList { get; set; }

        /// <summary>
        ///     Selected roles for user
        /// </summary>
        [DisplayName("Selected roles")]
        public IEnumerable<string> SelectedRoles { get; set; }
    }
}