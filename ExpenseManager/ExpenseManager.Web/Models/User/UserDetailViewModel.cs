using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Web.Models.User
{
    /// <summary>
    ///     Model for user detail screen
    /// </summary>
    public class UserDetailViewModel
    {
        /// <summary>
        ///     Id of user, is hidden on all pages
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Email address of user
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        ///     User name (First name + Last name)
        /// </summary>
        [DisplayName("User name")]
        public string UserName { get; set; }

        /// <summary>
        ///     List of roles assigned to user
        /// </summary>
        public IEnumerable<string> RolesList { get; set; }
    }
}