using System.Collections.Generic;
using ExpenseManager.Entity.Enums;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Users;

namespace ExpenseManager.Entity.Categories
{
    /// <summary>
    ///     Category for transaction
    /// </summary>
    public class Category : BaseEntity
    {
        public Category()
        {
            Transactions = new List<Transaction>();
        }

        /// <summary>
        ///     Description of the category
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Glyphicon name of the category's icon
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        ///     Name of the category
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Type of the category
        /// </summary>
        public CategoryType Type { get; set; }

        /// <summary>
        ///     Creator of this category
        /// </summary>
        public UserProfile User { get; set; }

        /// <summary>
        ///     Transactions that uses this category
        /// </summary>
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}