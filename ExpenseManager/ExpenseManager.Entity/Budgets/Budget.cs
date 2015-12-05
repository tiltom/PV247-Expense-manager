using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Users;

namespace ExpenseManager.Entity.Budgets
{
    public class Budget : BaseEntity
    {
        public Budget()
        {
            Transactions = new List<Transaction>();
            AccessRights = new List<BudgetAccessRight>();
        }

        /// <summary>
        ///     Name of budget
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Description of budget
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Date when budget started
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     Date when budget ended
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     Amount of money which can be spent
        /// </summary>
        public decimal Limit { get; set; }

        /// <summary>
        ///     Creator of budget
        /// </summary>
        [Required]
        public virtual UserProfile Creator { get; set; }

        /// <summary>
        ///     Transactions made in this budget
        /// </summary>
        public virtual ICollection<Transaction> Transactions { get; set; }

        /// <summary>
        ///     Access rights of users to this budget
        /// </summary>
        public virtual ICollection<BudgetAccessRight> AccessRights { get; set; }
    }
}