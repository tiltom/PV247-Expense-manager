using System;
using System.Collections.Generic;

namespace ExpenseManager.Entity
{
    public class User : BaseEntity
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreationDate { get; set; }
        public virtual Wallet PersonalWallet { get; set; }
        public virtual ICollection<WalletAccessRight> WalletAccessRights { get; set; }
        public virtual ICollection<BudgetAccessRight> BudgetAccessRights { get; set; }
        public virtual ICollection<Budget> CreatedBudgets { get; set; }
    }
}