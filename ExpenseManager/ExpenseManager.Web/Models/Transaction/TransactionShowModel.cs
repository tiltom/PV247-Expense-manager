using System;
using System.ComponentModel.DataAnnotations;
using ExpenseManager.Resources;

namespace ExpenseManager.Web.Models.Transaction
{
    /// <summary>
    ///     Model for viewing saved transactions
    /// </summary>
    public class TransactionShowModel
    {
        /// <summary>
        ///     Unique id of transaction
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        ///     Amount of money in transaction
        /// </summary>
        [Required]
        [Display(Name = "Amount", ResourceType = typeof (SharedResource))]
        public decimal Amount { get; set; }

        /// <summary>
        ///     Date when transaction occurred
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        [Display(Name = "Date", ResourceType = typeof (SharedResource))]
        public DateTime Date { get; set; }

        /// <summary>
        ///     Short description of transaction
        /// </summary>
        [Display(Name = "Description", ResourceType = typeof (SharedResource))]
        public string Description { get; set; }

        /// <summary>
        ///     Budget in which transaction belongs
        /// </summary>
        [Display(Name = "Budget", ResourceType = typeof (SharedResource))]
        public string BudgetName { get; set; }

        /// <summary>
        ///     Currency which was used for transaction
        /// </summary>
        [Display(Name = "Currency", ResourceType = typeof (SharedResource))]
        public string CurrencySymbol { get; set; }

        /// <summary>
        ///     Category in which transactions belongs
        /// </summary>
        [Display(Name = "Category", ResourceType = typeof (SharedResource))]
        public string CategoryName { get; set; }

        /// <summary>
        ///     Bool representing if transaction is repeatable
        /// </summary>
        [Display(Name = "RepeatableTransaction", ResourceType = typeof (SharedResource))]
        public bool IsRepeatable { get; set; }
    }
}