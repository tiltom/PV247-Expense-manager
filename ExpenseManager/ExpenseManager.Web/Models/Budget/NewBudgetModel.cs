using System;
using System.ComponentModel.DataAnnotations;
using ExpenseManager.Resources.BudgetResources;
using ExpenseManager.Web.Constants.BudgetConstants;

namespace ExpenseManager.Web.Models.Budget
{
    /// <summary>
    ///     Model for creating budget
    /// </summary>
    public class NewBudgetModel
    {
        /// <summary>
        ///     name of the new budget
        /// </summary>
        [Required]
        [Display(Name = "Name", ResourceType = typeof (BudgetResource))]
        public string Name { get; set; }

        /// <summary>
        ///     start date of budget
        /// </summary>
        [Required]
        [Display(Name = "StartDate", ResourceType = typeof (BudgetResource))]
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     end date of budget
        /// </summary>
        [Required]
        [Display(Name = "EndDate", ResourceType = typeof (BudgetResource))]
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     limit for the budget
        /// </summary>
        [Required]
        [Display(Name = "Limit", ResourceType = typeof (BudgetResource))]
        [Range(typeof (decimal), BudgetConstant.LimitMinimumValue, BudgetConstant.LimitMaximumValue,
            ErrorMessageResourceType = typeof (BudgetResource), ErrorMessageResourceName = "LimitGreaterThanZero")]
        public decimal Limit { get; set; }

        /// <summary>
        ///     description of the budget
        /// </summary>
        [Display(Name = "Description", ResourceType = typeof (BudgetResource))]
        public string Description { get; set; }
    }
}