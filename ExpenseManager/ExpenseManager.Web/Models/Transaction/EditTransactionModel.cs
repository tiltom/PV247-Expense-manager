using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Web.Models.Transaction
{
    public class EditTransactionModel : NewTransactionModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date")]
        public new DateTime Date { get; set; }
    }
}