using System.ComponentModel.DataAnnotations;
using ExpenseManager.Entity.Users;

namespace ExpenseManager.Entity.Wallets
{
    public class WalletAccessRight : BaseEntity
    {
        /// <summary>
        ///     Permission type
        /// </summary>
        [EnumDataType(typeof (PermissionEnum))]
        public PermissionEnum Permission { get; set; }

        /// <summary>
        ///     User for whom is this access right
        /// </summary>
        [Required]
        public virtual UserProfile UserProfile { get; set; }

        /// <summary>
        ///     Budget for which is this access right
        /// </summary>
        [Required]
        public virtual Wallet Wallet { get; set; }
    }
}