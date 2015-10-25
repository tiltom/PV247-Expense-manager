using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ExpenseManager.Entity.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ExpenseManager.Web.Common
{
    public class UserIdentity : IdentityUser
    {
        public virtual UserProfile Profile { get; set; }
        public DateTime CreationDate { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<UserIdentity> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom UserProfile claims here
            return userIdentity;
        }
    }
}