using System.Collections.Generic;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.Entity.Users
{
    public class UserProfile : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual Wallet PersonalWallet { get; set; }
        public virtual ICollection<WalletAccessRight> WalletAccessRights { get; set; }
        public virtual ICollection<BudgetAccessRight> BudgetAccessRights { get; set; }
        public virtual ICollection<Budget> CreatedBudgets { get; set; }
    }
}