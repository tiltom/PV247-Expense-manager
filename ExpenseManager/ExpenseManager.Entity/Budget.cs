using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManager.Entity
{
    public class Budget
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDay { get; set; }
        public int Duration { get; set; }
        public float Limit { get; set; }
        public int WalletId { get; set; }
        public virtual Wallet Wallet { get; set; }
        public virtual ICollection<AccessRight> AccessRights { get; set; }
    }
}