using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Web.Models.User
{
    public class UserDetailViewModel
    {
        public string Id { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("User name")]
        public string UserName { get; set; }


        public IEnumerable<string> RolesList { get; set; }
    }
}