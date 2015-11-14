using System.Data.Entity;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.Database.Contexts
{
    internal interface IUserContext : ICurrencyContext
    {
        DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<Wallet> Wallets { get; set; }
        DbSet<WalletAccessRight> WalletAccessRights { get; set; }
    }
}