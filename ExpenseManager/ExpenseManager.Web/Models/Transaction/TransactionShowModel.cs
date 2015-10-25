using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Web.Models.Transaction
{
    public class TransactionShowModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public string Description { get; set; }

        [Display(Name = "Budget")]
        public string BudgetName { get; set; }

        [Display(Name = "Currency")]
        public string CurrencySymbol { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Display(Name = "Repeatable transaction")]
        public bool IsRepeatable { get; set; }
    }
}