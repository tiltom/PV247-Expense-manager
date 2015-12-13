using System;
using System.ComponentModel.DataAnnotations;
using ExpenseManager.Resources.BudgetResources;
using ExpenseManager.Web.Constants;
using ExpenseManager.Web.Constants.BudgetConstants;

namespace ExpenseManager.Web.Models.Budget
{
    /// <summary>
    ///     Model of budget for editing purposes
    /// </summary>
    public class EditBudgetModel
    {
        /// <summary>
        ///     name of the budget
        /// </summary>
        [Required]
        [Display(Name = "Name", ResourceType = typeof (BudgetResource))]
        public string Name { get; set; }

        /// <summary>
        ///     start date of the budget
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = SharedConstant.DateFormatModels, ApplyFormatInEditMode = true)]
        [Display(Name = "StartDate", ResourceType = typeof (BudgetResource))]
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     end date of the budget
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = SharedConstant.DateFormatModels, ApplyFormatInEditMode = true)]
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

        /// <summary>
        ///     guid of the budget (without name because is hidden in form)
        /// </summary>
        public Guid Guid { get; set; }
    }
}