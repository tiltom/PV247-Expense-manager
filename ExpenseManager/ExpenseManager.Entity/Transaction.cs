using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManager.Entity
{
    public class Transaction
    {
        public enum RepeatTransactionEnum
        {
            Yes,
            No
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public float Price { get; set; }
        public string ReceiptImage { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }

        [EnumDataType(typeof (RepeatTransactionEnum))]
        public RepeatTransactionEnum RepeatTransaction { get; set; }

        public int WalletId { get; set; }
        public int BudgetId { get; set; }
        public int CategoryId { get; set; }
        public int? Frequency { get; set; }
        public DateTime? LastOccurence { get; set; }
        public virtual Wallet Wallet { get; set; }
        public virtual Budget Budget { get; set; }
        public virtual Category Category { get; set; }
    }
}