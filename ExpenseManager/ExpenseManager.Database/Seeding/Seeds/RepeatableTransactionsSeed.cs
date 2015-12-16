using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity.Enums;
using ExpenseManager.Entity.Transactions;

namespace ExpenseManager.Database.Seeding.Seeds
{
    internal class RepeatableTransactionsSeed<TContext> : ISeeds<TContext>
        where TContext : DbContext, ITransactionContext
    {
        public void Seed(TContext context)
        {
            var firstTransaction = context.Transactions.FirstOrDefault();

            var repeatableTransactions = new List<RepeatableTransaction>
            {
                new RepeatableTransaction
                {
                    FirstTransaction = context.Transactions.Where(t => t.Description == "Rent").FirstOrDefault(),
                    NextRepeat = 1,
                    FrequencyType = FrequencyType.Month,
                    LastOccurrence = DateTime.Now.AddDays(-40)
                }
            };

            context.RepeatableTransactions.AddRange(repeatableTransactions);
            context.SaveChanges();
        }
    }
}