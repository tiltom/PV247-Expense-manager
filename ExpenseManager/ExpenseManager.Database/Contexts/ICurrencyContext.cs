using System.Data.Entity;
using ExpenseManager.Entity.Currencies;

namespace ExpenseManager.Database.Contexts
{
    internal interface ICurrencyContext
    {
        DbSet<Currency> Currencies { get; set; }
    }
}