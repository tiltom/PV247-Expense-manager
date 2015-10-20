using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ExpenseManager.Entity;

namespace ExpenseManager.Web.Models.WalletAcessRight
{
    public class WalletAcessRightModel
    {
        public Guid Id { get; set; }

        [Display(Name = "User")]
        [Required]
        public string AssignedUserId { get; set; }

        public string AssignedUserName { get; set; }

        [Required]
        public Guid WalletId { get; set; }

        [Display(Name = "Assigned permission")]
        [Required]
        public PermissionEnum Permission { get; set; }

        public List<SelectListItem> Users { get; set; }
    }
}