using System;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        ///     start date of the budget
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     end date of the budget
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     limit for the budget
        /// </summary>
        [Required]
        [Display(Name = "Limit")]
        [Range(typeof (decimal), "0", "9999999999999999999999")]
        public decimal Limit { get; set; }

        /// <summary>
        ///     description of the budget
        /// </summary>
        [Display(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        ///     guid of the budget (without name because is hidden in form)
        /// </summary>
        public Guid Guid { get; set; }
    }
}