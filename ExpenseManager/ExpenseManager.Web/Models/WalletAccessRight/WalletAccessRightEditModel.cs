using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ExpenseManager.Web.Models.WalletAccessRight
{
    /// <summary>
    ///     Model representing wallet access rights for other users.
    ///     It contains option for permission dropdown and all fields required for managing of wallets.
    /// </summary>
    public class WalletAccessRightEditModel
    {
        /// <summary>
        ///     id of the wallet access right (hidden in all forms so without name attribute)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     id of the assigned user (selected from dropdown)
        /// </summary>
        [Display(Name = "User")]
        [Required]
        public Guid AssignedUserId { get; set; }

        /// <summary>
        ///     name of the user with this access right
        /// </summary>
        [Display(Name = "Assigned User")]
        public string AssignedUserName { get; set; }

        /// <summary>
        ///     id of the wallet with changed access rights
        /// </summary>
        [Required]
        public Guid WalletId { get; set; }

        /// <summary>
        ///     permission assigned to user
        /// </summary>
        [Display(Name = "Assigned permission")]
        [Required]
        public string Permission { get; set; }

        /// <summary>
        ///     list with options for dropdown (contains id and name of the permission)
        /// </summary>
        public List<SelectListItem> Permissions { get; set; }
    }
}