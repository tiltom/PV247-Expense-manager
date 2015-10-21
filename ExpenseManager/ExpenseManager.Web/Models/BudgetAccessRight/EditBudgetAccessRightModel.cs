using System;
using System.ComponentModel.DataAnnotations;
using ExpenseManager.Entity;

namespace ExpenseManager.Web.Models.BudgetAccessRight
{
    public class EditBudgetAccessRightModel
    {
        public Guid Id { get; set; }

        [Display(Name = "User")]
        [Required]
        public string AssignedUserId { get; set; }

        public string AssignedUserName { get; set; }

        [Required]
        [Display(Name = "Permission")]
        public PermissionEnum Permission { get; set; }

        [Required]
        public Guid BudgetId { get; set; }
    }
}