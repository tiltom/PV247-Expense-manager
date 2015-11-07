using ExpenseManager.Database.common;
using ExpenseManager.Web.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Web.Models.Role
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Role name")]
        public string Name { get; set; }

        public List<UserIdentity> Users { get; set; }
    }
}