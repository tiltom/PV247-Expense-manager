using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ExpenseManager.Resources.RolesAdmin;
using ExpenseManager.Web.Models.User;

namespace ExpenseManager.Web.Models.Role
{
    public class RoleDetailViewModel
    {
        public RoleDetailViewModel()
        {
            Users = Enumerable.Empty<UserDetailViewModel>();
        }

        public string Id { get; set; }

        [Required]
        [Display(ResourceType = typeof (RolesAdminResource), Name = "RoleName")]
        public string Name { get; set; }

        public IEnumerable<UserDetailViewModel> Users { get; set; }
    }
}