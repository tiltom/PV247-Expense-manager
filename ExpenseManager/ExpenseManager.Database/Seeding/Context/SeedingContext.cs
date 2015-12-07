using System.Data.Entity;
using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ExpenseManager.Database.Seeding.Context
{
    internal class SeedingContext : IdentityDbContext<UserIdentity>, IWalletContext, IUserContext, ITransactionContext,
        IBudgetContext
    {
        public SeedingContext()
            : base("DefaultConnection", false)
        {
        }

        public DbSet<BudgetAccessRight> BudgetAccessRights { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<RepeatableTransaction> RepeatableTransactions { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletAccessRight> WalletAccessRights { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
        }
    }
}