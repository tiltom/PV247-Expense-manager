using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Web.Models.Budget
{
    public class NewBudgetModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Limit")]
        public decimal Limit { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}