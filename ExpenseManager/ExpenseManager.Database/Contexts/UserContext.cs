using ExpenseManager.Database.common;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ExpenseManager.Database.Contexts
{
    public class UserContext : IdentityDbContext<UserIdentity>, IUserContext
    {
        public UserContext()
            : base("DefaultConnection", false)
        {
        }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<WalletAccessRight> WalletAccessRights { get; set; }

        public static UserContext Create()
        {
            return new UserContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
