using System.Collections.Generic;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.Entity.Users
{
    public class UserProfile : BaseEntity
    {
        /// <summary>
        ///     First name of user
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Last name of user
        /// </summary>
        public string LastName { get; set; }


        /// <summary>
        ///     Access rights to wallets
        /// </summary>
        public virtual ICollection<WalletAccessRight> WalletAccessRights { get; set; }

        /// <summary>
        ///     Access rights to budgets
        /// </summary>
        public virtual ICollection<BudgetAccessRight> BudgetAccessRights { get; set; }
    }
}