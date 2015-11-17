using System;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        ///     start date of budget
        /// </summary>
        [Required]
        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     end date of budget
        /// </summary>
        [Required]
        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     limit for the budget
        /// </summary>
        [Required]
        [Display(Name = "Limit")]
        [Range(typeof (decimal), "1", "9999999999999999999999",
            ErrorMessage = "Limit for budget must be greater than 0.")]
        public decimal Limit { get; set; }

        /// <summary>
        ///     description of the budget
        /// </summary>
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}