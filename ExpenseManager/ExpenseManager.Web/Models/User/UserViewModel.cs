using System.ComponentModel.DataAnnotations;
using ExpenseManager.Resources.UsersAdminResources;

namespace ExpenseManager.Web.Models.User
{
    /// <summary>
    ///     Model for viewing user on Index page
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        ///     Id of user, is hidden on all pages
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        ///     First name of user
        /// </summary>
        [Display(Name = "FirstName", ResourceType = typeof (UsersAdminResource))]
        public string FirstName { get; set; }

        /// <summary>
        ///     Last name of user
        /// </summary>
        [Display(Name = "LastName", ResourceType = typeof (UsersAdminResource))]
        public string LastName { get; set; }

        /// <summary>
        ///     Email address of user
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }
    }
}