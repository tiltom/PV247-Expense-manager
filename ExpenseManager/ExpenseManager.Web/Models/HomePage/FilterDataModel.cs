using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ExpenseManager.Web.Models.HomePage
{
    public class FilterDataModel
    {
        public FilterDataModel()
        {
            Wallets = new List<Guid>();
            Categories = new List<Guid>();
            Budgets = new List<Guid>();
            CategoriesSelectList = new List<SelectListItem>();
            WalletsSelectList = new List<SelectListItem>();
            BudgetsSelectList = new List<SelectListItem>();
        }


        /// <summary>
        ///     categories for selectection combo
        /// </summary>
        public IEnumerable<SelectListItem> CategoriesSelectList { get; set; }


        /// <summary>
        ///     selected categories
        /// </summary>
        public IEnumerable<Guid> Categories { get; set; }

        /// <summary>
        ///     wallets for selectection combo
        /// </summary>
        public IEnumerable<SelectListItem> WalletsSelectList { get; set; }

        /// <summary>
        ///     selected wallets
        /// </summary>
        public IEnumerable<Guid> Wallets { get; set; }

        /// <summary>
        ///     budgets for selectection combo
        /// </summary>
        public IEnumerable<SelectListItem> BudgetsSelectList { get; set; }

        /// <summary>
        ///     selected budgets
        /// </summary>
        public IEnumerable<Guid> Budgets { get; set; }

        public bool IsFilterSet()
        {
            return Budgets.Any() || Wallets.Any() || Categories.Any();
        }
    }
}