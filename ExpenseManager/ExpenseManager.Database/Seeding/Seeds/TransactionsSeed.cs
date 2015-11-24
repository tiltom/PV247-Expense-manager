using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity.Transactions;

namespace ExpenseManager.Database.Seeding.Seeds
{
    internal class TransactionsSeed<TContext> : ISeeds<TContext>
        where TContext : DbContext, ITransactionContext
    {
        public void Seed(TContext context)
        {
            var wallet = context.Wallets.FirstOrDefault();

            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    Wallet = wallet,
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = 20,
                    Date = new DateTime(2015, 10, 11),
                    Category = context.Categories.FirstOrDefault(x => x.Name.Contains("Salary")),
                    Description = "Found 20 euro on the ground"
                },
                new Transaction
                {
                    Wallet = wallet,
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = 10,
                    Date = new DateTime(2015, 10, 12),
                    Category = context.Categories.FirstOrDefault(),
                    Description = "Bought a ticket to the cinema"
                },
                new Transaction
                {
                    Wallet = wallet,
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = 5,
                    Date = new DateTime(2015, 10, 13),
                    Category = context.Categories.FirstOrDefault(),
                    Description = "Bet on a Chicago Blackhawks"
                },
                new Transaction
                {
                    Wallet = wallet,
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = 50,
                    Date = new DateTime(2015, 10, 14),
                    Category = context.Categories.FirstOrDefault(x => x.Description.Contains("transportation")),
                    Description = "Bought a ticket to Madrid"
                },
                new Transaction
                {
                    Wallet = context.Wallets.FirstOrDefault(u => u.Name.Contains("Read")),
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = 5,
                    Date = new DateTime(2015, 10, 15),
                    Category = context.Categories.FirstOrDefault(),
                    Description = "Read transaction"
                },
                new Transaction
                {
                    Wallet = context.Wallets.FirstOrDefault(u => u.Name.Contains("Write")),
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = 50,
                    Date = new DateTime(2015, 10, 16),
                    Category = context.Categories.FirstOrDefault(x => x.Description.Contains("transportation")),
                    Description = "Write transaction"
                },
                new Transaction
                {
                    Wallet = wallet,
                    Currency = context.Currencies.FirstOrDefault(x => x.Symbol == "Kč"),
                    Amount = 100000,
                    Date = new DateTime(2015, 11, 17),
                    Category = context.Categories.FirstOrDefault(x => x.Name == "Salary"),
                    Description = "Asp.net expense manager payment"
                },
                new Transaction
                {
                    Wallet = wallet,
                    Currency = context.Currencies.FirstOrDefault(x => x.Symbol == "Kč"),
                    Amount = 20000,
                    Date = new DateTime(2013, 11, 25),
                    Category = context.Categories.FirstOrDefault(x => x.Name == "Bitcoin"),
                    Description = "Bought too late"
                },
                new Transaction
                {
                    Wallet = wallet,
                    Currency = context.Currencies.FirstOrDefault(x => x.Symbol == "Kč"),
                    Amount = 4400,
                    Date = new DateTime(2015, 01, 12),
                    Category = context.Categories.FirstOrDefault(x => x.Name == "Bitcoin"),
                    Description = "Sold too soon"
                }
            };

            context.Transactions.AddRange(transactions);
            context.SaveChanges();
        }
    }
}