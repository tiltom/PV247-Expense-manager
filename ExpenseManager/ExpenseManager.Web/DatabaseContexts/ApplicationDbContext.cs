using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;
using ExpenseManager.Web.Common;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ExpenseManager.Web.DatabaseContexts
{
    public class ApplicationDbContext : IdentityDbContext<UserIdentity>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", false)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<BudgetAccessRight> BudgetAccessRights { get; set; }
        public DbSet<WalletAccessRight> WalletAccessRights { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<RepeatableTransaction> RepeatableTransactions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserIdentity>()
                .HasOptional(x => x.Profile)
                .WithRequired()
                .WillCascadeOnDelete();

            modelBuilder.Entity<UserProfile>()
                .HasOptional(x => x.PersonalWallet)
                .WithRequired(x => x.Owner)
                .WillCascadeOnDelete();

            modelBuilder.Entity<UserProfile>()
                .HasMany(x => x.CreatedBudgets)
                .WithRequired(x => x.Creator)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Wallet>()
                .HasMany(x => x.WalletAccessRights)
                .WithRequired(x => x.Wallet)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Wallet>()
                .HasMany(x => x.Transactions)
                .WithRequired(x => x.Wallet)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Budget>()
                .HasMany(x => x.AccessRights)
                .WithRequired(x => x.Budget)
                .WillCascadeOnDelete();


            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}