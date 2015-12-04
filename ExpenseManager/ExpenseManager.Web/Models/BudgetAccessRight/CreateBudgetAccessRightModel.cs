using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using ExpenseManager.Entity;

namespace ExpenseManager.Web.Models.BudgetAccessRight
{
    /// <summary>
    ///     model for the creation of the budget access right
    /// </summary>
    public class CreateBudgetAccessRightModel
    {
        /// <summary>
        /// Constructor for model with initialization of collections.
        /// </summary>
        public CreateBudgetAccessRightModel()
        {
            Users = Enumerable.Empty<SelectListItem>();
            Permissions = Enumerable.Empty<SelectListItem>();
        }

        /// <summary>
        ///     id of the user with this access right
        /// </summary>
        [Display(Name = "Assigned user")]
        [Required]
        public string AssignedUserId { get; set; }

        /// <summary>
        ///     name of the user with this access right
        /// </summary>
        public string AssignedUserName { get; set; }

        /// <summary>
        ///     permission level for the user
        /// </summary>
        [Required]
        [Display(Name = "Permission")]
        public PermissionEnum Permission { get; set; }

        /// <summary>
        ///     id of the managed budget
        /// </summary>
        [Required]
        public Guid BudgetId { get; set; }

        /// <summary>
        ///     list of users as options for dropdown
        /// </summary>
        public IEnumerable<SelectListItem> Users { get; set; }

        /// <summary>
        ///     list of permissions as options for dropdown
        /// </summary>
        public IEnumerable<SelectListItem> Permissions { get; set; }
    }
}