using System;
using System.Collections.Generic;

namespace ExpenseManager.BusinessLogic.DashboardServices.Models
{
    public class FilterDataServiceModel
    {
        /// <summary>
        ///     start of interval for transactions shown
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     end of interval for transactions shown
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     selected categories
        /// </summary>
        public IEnumerable<Guid> Categories { get; set; }


        /// <summary>
        ///     selected wallets
        /// </summary>
        public IEnumerable<Guid> Wallets { get; set; }

        /// <summary>
        ///     selected budgets
        /// </summary>
        public IEnumerable<Guid> Budgets { get; set; }


        public FilterDataServiceModel WithMonthFilterValues()
        {
            StartDate = DateTime.Now.AddMonths(-1);
            EndDate = DateTime.Now;
            return this;
        }

        public FilterDataServiceModel WithYearFilterValues()
        {
            StartDate = DateTime.Now.AddYears(-1);
            EndDate = DateTime.Now;
            return this;
        }
    }
}