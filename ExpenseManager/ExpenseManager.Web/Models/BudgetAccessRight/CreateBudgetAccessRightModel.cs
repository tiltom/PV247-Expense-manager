using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ExpenseManager.Entity;

namespace ExpenseManager.Web.Models.BudgetAccessRight
{
    public class CreateBudgetAccessRightModel
    {
        [Display(Name = "UserProfile")]
        [Required]
        public string AssignedUserId { get; set; }

        public string AssignedUserName { get; set; }

        [Required]
        [Display(Name = "Permission")]
        public PermissionEnum Permission { get; set; }

        [Required]
        public Guid BudgetId { get; set; }

        public List<SelectListItem> Users { get; set; }

        public List<SelectListItem> Permissions { get; set; }
    }
}