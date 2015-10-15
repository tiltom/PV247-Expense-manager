using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Entity
{
    public class Transaction : BaseEntity
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }

        [Required]
        public virtual Wallet Wallet { get; set; }

        public virtual Budget Budget { get; set; }

        [Required]
        public virtual Currency Currency { get; set; }

        [Required]
        public virtual Category Category { get; set; }
    }
}