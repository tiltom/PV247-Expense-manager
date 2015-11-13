using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.Database.Contexts
{
    internal interface IUserContext : ICurrencyContext
    {
        DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<Wallet> Wallets { get; set; }
        DbSet<WalletAccessRight> WalletAccessRights { get; set; }
    }
}
