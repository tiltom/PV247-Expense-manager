using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using ExpenseManager.Entity;

namespace ExpenseManager.Web.DatabaseContexts
{
    public class ExpenseManagerContext : DbContext
    {
        public ExpenseManagerContext() : base("DefaultConnection")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<BudgetAccessRight> BudgetAccessRights { get; set; }
        public DbSet<WalletAccessRight> WalletAccessRights { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<RepeatableTransaction> RepeatableTransactions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}