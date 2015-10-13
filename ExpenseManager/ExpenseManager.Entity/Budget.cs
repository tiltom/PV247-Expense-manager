using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManager.Entity
{
    public class Budget : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Limit { get; set; }

        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<BudgetAccessRight> AccessRights { get; set; }
    }
}