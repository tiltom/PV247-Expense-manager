﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExpenseManager.Database.Contexts;
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
                    Frequency = new TimeSpan(2, 0, 0),
                    LastOccurence = new DateTime(2015, 10, 17)
                }
            };

            context.RepeatableTransactions.AddRange(repeatableTransactions);
            context.SaveChanges();
        }
    }
}