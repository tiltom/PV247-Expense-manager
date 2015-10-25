using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ExpenseManager.Web.Models.WalletAcessRight
{
    public class WalletAcessRightModel
    {
        public Guid Id { get; set; }

        [Display(Name = "UserProfile")]
        [Required]
        public Guid AssignedUserId { get; set; }

        [Display(Name = "Assigned UserProfile")]
        public string AssignedUserName { get; set; }

        [Required]
        public Guid WalletId { get; set; }

        [Display(Name = "Assigned permission")]
        [Required]
        public string Permission { get; set; }

        public List<SelectListItem> Users { get; set; }

        public List<SelectListItem> Permissions { get; set; }
    }
}