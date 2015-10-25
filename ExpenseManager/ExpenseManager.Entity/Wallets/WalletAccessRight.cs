using System.ComponentModel.DataAnnotations;
using ExpenseManager.Entity.Users;

namespace ExpenseManager.Entity.Wallets
{
    public class WalletAccessRight : BaseEntity
    {
        [EnumDataType(typeof (PermissionEnum))]
        public PermissionEnum Permission { get; set; }

        [Required]
        public virtual UserProfile UserProfile { get; set; }

        [Required]
        public virtual Wallet Wallet { get; set; }
    }
}