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
                    FirstTransaction = firstTransaction,
                    NextRepeat = 2,
                    FrequencyType = FrequencyType.Monthly,
                    LastOccurrence = new DateTime(2015, 10, 17)
                }
            };

            context.RepeatableTransactions.AddRange(repeatableTransactions);
            context.SaveChanges();
        }
    }
}