using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ExpenseManager.Resources;
using ExpenseManager.Resources.WalletResources;

namespace ExpenseManager.Web.Models.WalletAccessRight
{
    /// <summary>
    ///     Model representing wallet access rights for other users.
    ///     It contains some options for dropdowns and all fields required for managing of wallets.
    /// </summary>
    public class WalletAccessRightModel
    {
        /// <summary>
        ///     id of the wallet access right (hidden in all forms so without name attribute)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     id of the assigned user (selected from dropdown)
        /// </summary>
        [Display(ResourceType = typeof (SharedResource), Name = "UserEmail")]
        [Required]
        [EmailAddress]
        public string AssignedUserEmail { get; set; }

        /// <summary>
        ///     name of the user with this access right
        /// </summary>
        [Display(ResourceType = typeof (WalletAccessRightResource), Name = "AssignedUserName")]
        public string AssignedUserName { get; set; }

        /// <summary>
        ///     id of the wallet with changed access rights
        /// </summary>
        [Required]
        public Guid WalletId { get; set; }

        /// <summary>
        ///     permission assigned to user
        /// </summary>
        [Display(ResourceType = typeof (WalletAccessRightResource), Name = "AssignedPermission")]
        [Required]
        public string Permission { get; set; }


        /// <summary>
        ///     list with options for dropdown (contains id and name of the permission)
        /// </summary>
        public List<SelectListItem> Permissions { get; set; }
    }
}