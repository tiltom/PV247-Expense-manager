using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManager.Entity
{
    public class Wallet : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual User Owner { get; set; }
        public virtual ICollection<User> UsersWithReadAccess { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}