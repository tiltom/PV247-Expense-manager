using System.ComponentModel.DataAnnotations;
using ExpenseManager.Entity.Users;

namespace ExpenseManager.Entity.Budgets
{
    public class BudgetAccessRight : BaseEntity
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
        public virtual Budget Budget { get; set; }
    }
}