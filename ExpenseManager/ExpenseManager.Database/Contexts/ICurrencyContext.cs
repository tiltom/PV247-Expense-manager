using ExpenseManager.Entity.Currencies;
using System.Data.Entity;

namespace ExpenseManager.Database.Contexts
{
    internal interface ICurrencyContext
    {
        DbSet<Currency> Currencies { get; set; }
    }
}
