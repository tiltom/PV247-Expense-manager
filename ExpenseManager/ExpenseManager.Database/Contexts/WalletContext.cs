using System.Data.Entity;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.Database.Contexts
{
    public class WalletContext : DbContext, IWalletContext
    {
        public WalletContext() 
            : base("DefaultConnection")
        {
        }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletAccessRight> WalletAccessRights { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        
    }
}
