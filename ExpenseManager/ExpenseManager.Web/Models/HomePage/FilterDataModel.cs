using System;
using System.Collections.Generic;

namespace ExpenseManager.Web.Models.HomePage
{
    public class FilterDataModel
    {
        public FilterDataModel()
        {
            Wallets = new List<Guid>();
            Categories = new List<Guid>();
            Budgets = new List<Guid>();
        }


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
    }
}