using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ExpenseManager.Web.Models.Transaction
{
    public class TransactionModel
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

        [Required]
        public Guid WalletId { get; set; }

        public virtual Entity.Budgets.Budget Budget { get; set; }

        [Display(Name = "Currency")]
        [Required]
        public string CurrencyId { get; set; }

        [Display(Name = "Currency")]
        public string CurrencySymbol { get; set; }

        [Display(Name = "Category")]
        [Required]
        public string CategoryId { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Display(Name = "Select Category")]
        public List<SelectListItem> Categories { get; set; }

        [Display(Name = "Select Currency")]
        public List<SelectListItem> Currencies { get; set; }

        public List<SelectListItem> Budgets { get; set; }

        [Display(Name = "Repeat transaction?")]
        public bool IsRepeatable { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan Frequency { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Last Ocurence")]
        public DateTime? LastOccurence { get; set; }
    }
}