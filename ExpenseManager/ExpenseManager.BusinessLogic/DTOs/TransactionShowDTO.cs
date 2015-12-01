using System;

namespace ExpenseManager.BusinessLogic.DTOs
{
    public class TransactionShowDTO
    {
        /// <summary>
        ///     Unique id of transaction
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Amount of money in transaction
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        ///     Date when transaction occurred
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///     Short description of transaction
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Budget in which transaction belongs
        /// </summary>
        public string BudgetName { get; set; }

        /// <summary>
        ///     Id of budget where transaction belongs
        /// </summary>
        public Guid? BudgetId { get; set; }

        /// <summary>
        ///     Currency which was used for transaction
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        ///     Category in which transactions belongs
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        ///     Id of category where transaction belongs
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        ///     Bool representing if transaction is repeatable
        /// </summary>
        public bool IsRepeatable { get; set; }
    }
}