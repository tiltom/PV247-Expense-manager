using System;
using System.ComponentModel.DataAnnotations;
using ExpenseManager.Entity;

namespace ExpenseManager.Web.Models.BudgetAccessRight
{
    public class ShowBudgetAccessRightModel
    {
        public Guid Id { get; set; }

        [Display(Name = "User")]
        public string AssignedUserName { get; set; }

        [Display(Name = "Permission")]
        public PermissionEnum Permission { get; set; }

        public Guid BudgetId { get; set; }
    }
}