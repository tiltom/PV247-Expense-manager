using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManager.Entity
{
    public class Transaction : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public decimal Amount { get; set; }
        public string ReceiptImage { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }

        public int WalletId { get; set; }
        public virtual Wallet Wallet { get; set; }

        public int? BudgetId { get; set; }
        public virtual Budget Budget { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}