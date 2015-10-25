﻿using System.ComponentModel.DataAnnotations;
using ExpenseManager.Entity.Users;

namespace ExpenseManager.Entity.Budgets
{
    public class BudgetAccessRight : BaseEntity
    {
        [EnumDataType(typeof (PermissionEnum))]
        public PermissionEnum Permission { get; set; }

        [Required]
        public virtual UserProfile UserProfile { get; set; }

        [Required]
        public virtual Budget Budget { get; set; }
    }
}