using System;
using System.Collections.Generic;

namespace ExpenseManager.BusinessLogic.DataTransferObjects
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreateTime { get; set; }
        public int? PersonalWalletId { get; set; }
        public virtual WalletDTO PersonalWallet { get; set; }
        public virtual ICollection<WalletDTO> AccessibleWallets { get; set; }
        public virtual ICollection<BudgetAccessRightDTO> AccessRightsToBudgets { get; set; }
        public virtual ICollection<BudgetDTO> CreatedBudgets { get; set; }
    }
}