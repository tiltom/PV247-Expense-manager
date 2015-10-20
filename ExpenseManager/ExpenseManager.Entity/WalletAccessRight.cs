using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Entity
{
    public class WalletAccessRight : BaseEntity
    {
        [EnumDataType(typeof (PermissionEnum))]
        public PermissionEnum Permission { get; set; }

        public virtual User User { get; set; }
        public virtual Wallet Wallet { get; set; }
    }
}