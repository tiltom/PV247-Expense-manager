using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ExpenseManager.Web.Models.Wallet
{
    /// <summary>
    ///     Model for managing of the wallet - user can change name of the wallet and currency of the wallet
    /// </summary>
    public class WalletEditModel
    {
        /// <summary>
        ///     id of the wallet - hidden on all pages so without name
        /// </summary>
        [Required]
        public Guid Guid { get; set; }

        /// <summary>
        ///     Name of the wallet
        /// </summary>
        [Display(Name = "Wallet Name")]
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     Currency of the wallet
        /// </summary>
        [Display(Name = "Currency")]
        [Required]
        public Guid CurrencyId { get; set; }

        /// <summary>
        ///     List of currencies available for the user
        /// </summary>
        public List<SelectListItem> Currencies { get; set; }
    }
}