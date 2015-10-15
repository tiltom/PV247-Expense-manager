﻿using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Entity
{
    public class BudgetAccessRight : BaseEntity
    {
        [EnumDataType(typeof (PermissionEnum))]
        public PermissionEnum Permission { get; set; }

        [Required]
        public virtual User User { get; set; }

        [Required]
        public virtual Budget Budget { get; set; }
    }
}