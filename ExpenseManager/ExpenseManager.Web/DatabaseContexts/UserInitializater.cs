using System.Collections.Generic;
using System.Data.Entity;
using ExpenseManager.Entity;

namespace ExpenseManager.Web.DatabaseContexts
{
    public class UserInitializater : DropCreateDatabaseIfModelChanges<UserDbContext>
    {
        protected override void Seed(UserDbContext context)
        {
            var currencies = new List<Currency>
            {
                new Currency
                {
                    Name = "American Dollar",
                    Symbol = "$"
                },
                new Currency
                {
                    Name = "Česká koruna",
                    Symbol = "Kč"
                },
                new Currency
                {
                    Name = "Euro",
                    Symbol = "€"
                }
            };

            context.Currencies.AddRange(currencies);

            context.SaveChanges();
        }
    }
}