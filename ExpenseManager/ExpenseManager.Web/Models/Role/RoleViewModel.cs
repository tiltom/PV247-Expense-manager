using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ExpenseManager.Database;
using ExpenseManager.Resources.RolesAdmin;

namespace ExpenseManager.Web.Models.Role
{
    public class RoleViewModel
    {
        public RoleViewModel()
        {
            Users = Enumerable.Empty<UserIdentity>();
        }

        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(ResourceType = typeof (RolesAdminResource), Name = "RoleName")]
        public string Name { get; set; }

        public IEnumerable<UserIdentity> Users { get; set; }
    }
}