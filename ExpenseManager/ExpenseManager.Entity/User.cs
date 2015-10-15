using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ExpenseManager.Entity
{
    public class User : IdentityUser
    {
        public DateTime CreationDate { get; set; }
        public virtual Wallet PersonalWallet { get; set; }
        public virtual ICollection<WalletAccessRight> WalletAccessRights { get; set; }
        public virtual ICollection<BudgetAccessRight> BudgetAccessRights { get; set; }
        public virtual ICollection<Budget> CreatedBudgets { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}