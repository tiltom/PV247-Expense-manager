using System;
using System.ComponentModel.DataAnnotations;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.Entity.Transactions
{
    /// <summary>
    ///     Entity representing one transaction
    /// </summary>
    public class Transaction : BaseEntity
    {
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
        ///     Wallet in which transaction belong
        /// </summary>
        [Required]
        public virtual Wallet Wallet { get; set; }

        /// <summary>
        ///     Budget in which transaction belongs
        /// </summary>
        public virtual Budget Budget { get; set; }

        /// <summary>
        ///     Currency which was used for transaction
        /// </summary>
        [Required]
        public virtual Currency Currency { get; set; }

        /// <summary>
        ///     Category in which transactions belongs
        /// </summary>
        [Required]
        public virtual Category Category { get; set; }
    }
}