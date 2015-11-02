using System.Collections.Generic;
using ExpenseManager.Entity.Transactions;

namespace ExpenseManager.Entity.Categories
{
    public class Category : BaseEntity
    {
        public Category()
        {
            Transactions = new List<Transaction>();
        }

        /// <summary>
        ///     Name of the category
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Description of the category
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Glyphicon name of the category's icon
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        ///     Transactions that uses this category
        /// </summary>
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}