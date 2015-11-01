using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity.Transactions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

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
                    Date = new DateTime(2015, 10, 17),
                    Category = context.Categories.FirstOrDefault(),
                    Description = "Found 20 euro on the ground"
                },
                new Transaction
                {
                    Wallet = wallet,
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = -10,
                    Date = new DateTime(2015, 10, 17),
                    Category = context.Categories.FirstOrDefault(),
                    Description = "Bought a ticket to the cinema"
                },
                new Transaction
                {
                    Wallet = wallet,
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = -5,
                    Date = new DateTime(2015, 10, 17),
                    Category = context.Categories.FirstOrDefault(),
                    Description = "Bet on a Chicago Blackhawks"
                },
                new Transaction
                {
                    Wallet = wallet,
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = -50,
                    Date = new DateTime(2015, 10, 16),
                    Category = context.Categories.FirstOrDefault(x => x.Description.Contains("transportation")),
                    Description = "Bought a ticket to Madrid"
                },
                new Transaction
                {
                    Wallet = context.Wallets.FirstOrDefault(u => u.Name.Contains("Read")),
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = -5,
                    Date = new DateTime(2015, 10, 17),
                    Category = context.Categories.FirstOrDefault(),
                    Description = "Read transaction"
                },
                new Transaction
                {
                    Wallet = context.Wallets.FirstOrDefault(u => u.Name.Contains("Write")),
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = -50,
                    Date = new DateTime(2015, 10, 16),
                    Category = context.Categories.FirstOrDefault(x => x.Description.Contains("transportation")),
                    Description = "Write transaction"
                }
            };

            context.Transactions.AddRange(transactions);
            context.SaveChanges();
        }
    }
}
