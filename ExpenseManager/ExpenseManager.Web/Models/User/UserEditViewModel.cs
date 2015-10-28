using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ExpenseManager.Web.Models.User
{
    public class UserEditViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }

        [DisplayName("Selected roles")]
        public IEnumerable<string> SelectedRoles { get; set; }
    }
}