using System.Collections.Generic;

namespace ExpenseManager.Entity
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