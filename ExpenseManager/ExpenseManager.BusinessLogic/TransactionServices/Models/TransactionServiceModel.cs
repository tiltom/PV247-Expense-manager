using System;
using ExpenseManager.Entity.Enums;

namespace ExpenseManager.BusinessLogic.TransactionServices.Models
{
    public class TransactionServiceModel
    {
        /// <summary>
        ///     Unique id of transaction
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Bool representing if transaction is expense
        /// </summary>
        public bool Expense { get; set; }

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
        ///     Id of wallet where transaction belongs
        /// </summary>
        public Guid WalletId { get; set; }

        /// <summary>
        ///     Id of budget where transaction belongs
        /// </summary>
        public Guid? BudgetId { get; set; }

        /// <summary>
        ///     Id of currency which was used for transaction
        /// </summary>
        public Guid CurrencyId { get; set; }

        /// <summary>
        ///     Id of category where transaction belongs
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        ///     Bool representing if transaction is repeatable
        /// </summary>
        public bool IsRepeatable { get; set; }

        /// <summary>
        ///     How often should transaction repeat
        /// </summary>
        public int? NextRepeat { get; set; }

        /// <summary>
        ///     Type of repetition
        /// </summary>
        public FrequencyType FrequencyType { get; set; }

        /// <summary>
        ///     Date until which transaction should repeat
        /// </summary>
        public DateTime? LastOccurrence { get; set; }
    }
}