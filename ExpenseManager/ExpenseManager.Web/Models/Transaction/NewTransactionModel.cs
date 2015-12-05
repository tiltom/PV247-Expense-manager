using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using ExpenseManager.Entity.Enums;

namespace ExpenseManager.Web.Models.Transaction
{
    /// <summary>
    ///     Model for adding new transaction
    /// </summary>
    public class NewTransactionModel
    {
        /// <summary>
        ///     Constructor for model with initialization of collections.
        /// </summary>
        public NewTransactionModel()
        {
            Categories = Enumerable.Empty<SelectListItem>();
            Currencies = Enumerable.Empty<SelectListItem>();
            Budgets = Enumerable.Empty<SelectListItem>();
        }

        /// <summary>
        ///     Bool representing if transaction is expense
        /// </summary>
        //[Required]
        public bool Expense { get; set; }

        /// <summary>
        ///     Amount of money in transaction
        /// </summary>
        //[Required]
        public decimal Amount { get; set; }

        /// <summary>
        ///     Date when transaction occurred
        /// </summary>
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        /// <summary>
        ///     Short description of transaction
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Id of wallet where transaction belongs
        /// </summary>
        //[Required]
        public Guid WalletId { get; set; }

        /// <summary>
        ///     Id of budget where transaction belongs
        /// </summary>
        [Display(Name = "Budget")]
        public Guid? BudgetId { get; set; }

        /// <summary>
        ///     Id of currency which was used for transaction
        /// </summary>
        [Display(Name = "Currency")]
        //[Required]
        public Guid CurrencyId { get; set; }

        /// <summary>
        ///     Id of category where transaction belongs
        /// </summary>
        [Display(Name = "Category")]
        //[Required]
        public Guid CategoryId { get; set; }

        /// <summary>
        ///     Bool representing if transaction is repeatable
        /// </summary>
        [Display(Name = "Repeat transaction?")]
        public bool IsRepeatable { get; set; }

        /// <summary>
        ///     How often should transaction repeat
        /// </summary>
        [Display(Name = "Repeat every")]
        public int? NextRepeat { get; set; }

        /// <summary>
        ///     Type of repetition
        /// </summary>
        public FrequencyType FrequencyType { get; set; }

        /// <summary>
        ///     Date until which transaction should repeat
        /// </summary>
        [Display(Name = "Repeat until")]
        public DateTime? LastOccurrence { get; set; }

        /// <summary>
        ///     List of all available Categories
        /// </summary>
        public IEnumerable<SelectListItem> Categories { get; set; }

        /// <summary>
        ///     List of all available Currencies
        /// </summary>
        public IEnumerable<SelectListItem> Currencies { get; set; }

        /// <summary>
        ///     List of all available Budgets
        /// </summary>
        public IEnumerable<SelectListItem> Budgets { get; set; }
    }
}