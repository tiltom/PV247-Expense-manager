using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
        [DisplayName("First name")]
        public string FirstName { get; set; }

        /// <summary>
        ///     Last name of user
        /// </summary>
        [DisplayName("Last name")]
        public string LastName { get; set; }

        /// <summary>
        ///     Email address of user
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }
    }
}