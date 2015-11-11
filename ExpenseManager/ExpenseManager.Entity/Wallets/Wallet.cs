using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Users;

namespace ExpenseManager.Entity.Wallets
{
    public class Wallet : BaseEntity
    {
        public Wallet()
        {
            WalletAccessRights = new List<WalletAccessRight>();
            Transactions = new List<Transaction>();
        }

        public string Name { get; set; }

        [Required]
        public virtual Currency Currency { get; set; }

        [Required]
        public virtual UserProfile Owner { get; set; }

        public virtual ICollection<WalletAccessRight> WalletAccessRights { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}