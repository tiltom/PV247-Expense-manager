using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ExpenseManager.Web.Models.Wallet
{
    public class WalletEditModel
    {
        [Required]
        public Guid Guid { get; set; }
        [Display(Name = "Wallet Name")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Currency")]
        [Required]
        public Guid CurrencyId { get; set; }
        public List<SelectListItem> Currencies { get; set; }

    }

   
}