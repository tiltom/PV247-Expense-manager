using System;
using System.ComponentModel.DataAnnotations;

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
        public decimal Amount { get; set; }

        /// <summary>
        ///     Date when transaction occurred
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        /// <summary>
        ///     Short description of transaction
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Budget in which transaction belongs
        /// </summary>
        [Display(Name = "Budget")]
        public string BudgetName { get; set; }

        /// <summary>
        ///     Currency which was used for transaction
        /// </summary>
        [Display(Name = "Currency")]
        public string CurrencySymbol { get; set; }

        /// <summary>
        ///     Category in which transactions belongs
        /// </summary>
        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        /// <summary>
        ///     Bool representing if transaction is repeatable
        /// </summary>
        [Display(Name = "Repeatable transaction")]
        public bool IsRepeatable { get; set; }
    }
}