using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Entity
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
        public virtual User Owner { get; set; }

        public virtual ICollection<WalletAccessRight> WalletAccessRights { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}