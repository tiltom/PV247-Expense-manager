using ExpenseManager.Database.common;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System;
using System.Data.Entity.ModelConfiguration.Conventions;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Budgets;

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
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<BudgetAccessRight>()
                .HasRequired(right => right.UserProfile)
                .WithMany(profile => profile.BudgetAccessRights)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WalletAccessRight>()
                .HasRequired(right => right.Wallet)
                .WithMany(w => w.WalletAccessRights)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Transaction>()
                .HasRequired(transaction => transaction.Wallet)
                .WithMany(wallet => wallet.Transactions)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Wallet>()
                .HasRequired(w => w.Owner)
                .WithOptional(o => o.PersonalWallet)
                .Map(m => m.MapKey("Owner_Guid"))
                .WillCascadeOnDelete(true);
        }
    }
}
