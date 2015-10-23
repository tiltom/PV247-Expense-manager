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

        public string Name { get; set; }
        public string Description { get; set; }
        public string IconPath { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}