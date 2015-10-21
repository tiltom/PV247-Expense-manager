﻿using System;
using System.Collections.Generic;

namespace ExpenseManager.Entity
{
    public class Budget : BaseEntity
    {
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