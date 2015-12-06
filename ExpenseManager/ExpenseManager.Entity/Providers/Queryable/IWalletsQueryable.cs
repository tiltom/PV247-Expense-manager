using System.Linq;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.Entity.Providers.Queryable
{
    public interface IWalletsQueryable
    {
        IQueryable<Wallet> Wallets { get; }
    }
}