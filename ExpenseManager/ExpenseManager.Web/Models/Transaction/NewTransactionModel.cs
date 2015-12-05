using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using ExpenseManager.Entity.Enums;
using ExpenseManager.Resources;
using ExpenseManager.Resources.TransactionResources;

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
        public bool Expense { get; set; }

        /// <summary>
        ///     Amount of money in transaction
        /// </summary>
        [Display(Name = "Amount", ResourceType = typeof (SharedResource))]
        public decimal Amount { get; set; }

        /// <summary>
        ///     Date when transaction occurred
        /// </summary>
        [Display(Name = "Date", ResourceType = typeof (SharedResource))]
        public DateTime Date { get; set; }

        /// <summary>
        ///     Short description of transaction
        /// </summary>
        [Display(Name = "Description", ResourceType = typeof (SharedResource))]
        public string Description { get; set; }

        /// <summary>
        ///     Id of wallet where transaction belongs
        /// </summary>
        [Display(Name = "Wallet", ResourceType = typeof (SharedResource))]
        public Guid WalletId { get; set; }

        /// <summary>
        ///     Id of budget where transaction belongs
        /// </summary>
        [Display(Name = "Budget", ResourceType = typeof (SharedResource))]
        public Guid? BudgetId { get; set; }

        /// <summary>
        ///     Id of currency which was used for transaction
        /// </summary>
        [Display(Name = "Currency", ResourceType = typeof (SharedResource))]
        public Guid CurrencyId { get; set; }

        /// <summary>
        ///     Id of category where transaction belongs
        /// </summary>
        [Display(Name = "Category", ResourceType = typeof (SharedResource))]
        public Guid CategoryId { get; set; }

        /// <summary>
        ///     Bool representing if transaction is repeatable
        /// </summary>
        [Display(Name = "IsRepeatable", ResourceType = typeof (TransactionResource))]
        public bool IsRepeatable { get; set; }

        /// <summary>
        ///     How often should transaction repeat
        /// </summary>
        [Display(Name = "NextRepeat", ResourceType = typeof (TransactionResource))]
        public int? NextRepeat { get; set; }

        /// <summary>
        ///     Type of repetition
        /// </summary>
        [Display(Name = "Frequency", ResourceType = typeof (TransactionResource))]
        public FrequencyType FrequencyType { get; set; }

        /// <summary>
        ///     Date until which transaction should repeat
        /// </summary>
        [Display(Name = "LastOccurrence", ResourceType = typeof (TransactionResource))]
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