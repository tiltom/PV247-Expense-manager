using ExpenseManager.Database.common;
using ExpenseManager.Web.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Web.Models.Role
{
    public class RoleDetailViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Role name")]
        public string Name { get; set; }

        public List<UserIdentity> Users { get; set; }
    }
}