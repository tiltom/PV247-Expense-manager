using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ExpenseManager.Web.Models.Transaction
{
    public class EditTransactionModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        public string Description { get; set; }

        [Required]
        public Guid WalletId { get; set; }

        [Display(Name = "Budget")]
        public string BudgetId { get; set; }

        [Display(Name = "Currency")]
        [Required]
        public string CurrencyId { get; set; }

        [Display(Name = "Category")]
        [Required]
        public string CategoryId { get; set; }

        public List<SelectListItem> Categories { get; set; }

        public List<SelectListItem> Currencies { get; set; }

        public List<SelectListItem> Budgets { get; set; }

        [Display(Name = "Repeat transaction?")]
        public bool IsRepeatable { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan Frequency { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Last Ocurence")]
        public DateTime? LastOccurence { get; set; }
    }
}