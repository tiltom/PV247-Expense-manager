using System;
using System.ComponentModel.DataAnnotations;
using ExpenseManager.Entity;

namespace ExpenseManager.Web.Models.BudgetAccessRight
{
    /// <summary>
    ///     Simplified model for displaying budget access rights on front end
    /// </summary>
    public class ShowBudgetAccessRightModel
    {
        /// <summary>
        ///     id of the budget access right - mostly hidden on front end so without name
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     username of the user with given permission
        /// </summary>
        [Display(Name = "User")]
        public string AssignedUserName { get; set; }

        /// <summary>
        ///     string representing permission of the user
        /// </summary>
        [Display(Name = "Permission")]
        public PermissionEnum Permission { get; set; }

        /// <summary>
        ///     id of the budget with given permissions
        /// </summary>
        public Guid BudgetId { get; set; }
    }
}