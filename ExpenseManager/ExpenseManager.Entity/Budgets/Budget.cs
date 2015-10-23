using System;
using System.Collections.Generic;
using ExpenseManager.Entity.Currencies;
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

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Limit { get; set; }

        public virtual User Creator { get; set; }
        public virtual Currency Currency { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<BudgetAccessRight> AccessRights { get; set; }
    }
}