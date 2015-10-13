using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManager.Entity
{
    public class User : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreateTime { get; set; }
        public int? PersonalWalletId { get; set; }
        public virtual Wallet PersonalWallet { get; set; }
        public virtual ICollection<Wallet> AccessibleWallets { get; set; }
        public virtual ICollection<BudgetAccessRight> AccessRightsToBudgets { get; set; }
        public virtual ICollection<Budget> CreatedBudgets { get; set; }
    }
}