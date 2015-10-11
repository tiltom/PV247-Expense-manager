using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManager.Entity
{
    public class AccessRight
    {
        public enum PermissionEnum
        {
            Read,
            Write
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [EnumDataType(typeof (PermissionEnum))]
        public PermissionEnum Permission { get; set; }

        public int UserId { get; set; }
        public int BudgetId { get; set; }
        public virtual User User { get; set; }
        public virtual Budget Budget { get; set; }
    }
}