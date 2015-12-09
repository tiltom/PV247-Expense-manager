using System;
using System.ComponentModel.DataAnnotations;
using ExpenseManager.Resources.BudgetResources;
using ExpenseManager.Web.Constants;

namespace ExpenseManager.Web.Models.Budget
{
    /// <summary>
    ///     Simplified model for showing of budget
    /// </summary>
    public class BudgetShowModel
    {
        /// <summary>
        ///     id of the budget - often hidden so without name
        /// </summary>
        [Required]
        public Guid Guid { get; set; }

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
        [DisplayFormat(DataFormatString = SharedConstant.DateFormatModels)]
        [Display(Name = "StartDate", ResourceType = typeof (BudgetResource))]
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     end date of the budget
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = SharedConstant.DateFormatModels)]
        [Display(Name = "EndDate", ResourceType = typeof (BudgetResource))]
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     Limit of the budget
        /// </summary>
        [Required]
        [Display(Name = "Limit", ResourceType = typeof (BudgetResource))]
        public decimal Limit { get; set; }
    }
}