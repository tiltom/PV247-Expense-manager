using System.Data.Entity;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.Database.Contexts
{
    internal interface IWalletContext : ICurrencyContext
    {
        DbSet<Wallet> Wallets { get; set; }
        DbSet<WalletAccessRight> WalletAccessRights { get; set; }
        DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<Transaction> Transactions { get; set; }
    }
}