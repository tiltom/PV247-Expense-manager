using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Web.Models.User
{
    public class UserViewModel
    {
        [Required]
        public string Id { get; set; }

        [DisplayName("First name")]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}