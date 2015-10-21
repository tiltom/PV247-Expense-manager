using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Web.Models.Budget
{
    public class BudgetShowModel
    {
        [Required]
        public Guid Guid { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Limit")]
        public decimal Limit { get; set; }
    }
}