using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Web.Models.HomePage
{
    public class FilterDataModel
    {
        public FilterDataModel()
        {
            StartDate = DateTime.Now.AddDays(-30);
            EndDate = DateTime.Now;
            Walets = new List<Guid>();
            Categories = new List<Guid>();
            Budgets = new List<Guid>();
        }

        /// <summary>
        ///     data used for graph generation
        /// </summary>
        /// <summary>
        ///     start of interval for transactions shown
        /// </summary>
        [Display(Name = "Start")]
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     end of interval for transactions shown
        /// </summary>
        [Display(Name = "End")]
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     selected categories
        /// </summary>
        public IEnumerable<Guid> Categories { get; set; }


        /// <summary>
        ///     selected wallets
        /// </summary>
        public IEnumerable<Guid> Walets { get; set; }

        /// <summary>
        ///     selected budgets
        /// </summary>
        public IEnumerable<Guid> Budgets { get; set; }
    }
}