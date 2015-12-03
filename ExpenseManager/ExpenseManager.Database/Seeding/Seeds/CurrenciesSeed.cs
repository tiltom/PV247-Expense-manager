using System.Collections.Generic;
using System.Data.Entity;
using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity.Currencies;

namespace ExpenseManager.Database.Seeding.Seeds
{
    internal class CurrenciesSeed<TContext> : ISeeds<TContext>
        where TContext : DbContext, ITransactionContext
    {
        public void Seed(TContext context)
        {
            var currencies = new List<Currency>
            {
                new Currency
                {
                    Name = "American Dollar",
                    Symbol = "$",
                    Code = "USD"
                },
                new Currency
                {
                    Name = "Česká koruna",
                    Symbol = "Kč",
                    Code = "CZK"
                },
                new Currency
                {
                    Name = "Euro",
                    Symbol = "€",
                    Code = "EUR"
                }
            };

            context.Currencies.AddRange(currencies);
            context.SaveChanges();
        }
    }
}