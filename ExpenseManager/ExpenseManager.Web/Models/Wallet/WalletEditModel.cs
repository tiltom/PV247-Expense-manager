using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using ExpenseManager.Resources;
using ExpenseManager.Resources.WalletResources;

namespace ExpenseManager.Web.Models.Wallet
{
    /// <summary>
    ///     Model for managing of the wallet - user can change name of the wallet and currency of the wallet
    /// </summary>
    public class WalletEditModel
    {
        public WalletEditModel()
        {
            Currencies = Enumerable.Empty<SelectListItem>();
        }

        /// <summary>
        ///     id of the wallet - hidden on all pages so without name
        /// </summary>
        [Required]
        public Guid Guid { get; set; }

        /// <summary>
        ///     Name of the wallet
        /// </summary>
        [Display(Name = "WalletName", ResourceType = typeof (WalletResource))]
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     Currency of the wallet
        /// </summary>
        [Display(Name = "Currency", ResourceType = typeof (SharedResource))]
        [Required]
        public Guid CurrencyId { get; set; }

        /// <summary>
        ///     List of currencies available for the user
        /// </summary>
        public IEnumerable<SelectListItem> Currencies { get; set; }
    }
}