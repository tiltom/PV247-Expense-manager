using System;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        ///     start date of the budget
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     end date of the budget
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     Limit of the budget
        /// </summary>
        [Required]
        [Display(Name = "Limit")]
        public decimal Limit { get; set; }
    }
}