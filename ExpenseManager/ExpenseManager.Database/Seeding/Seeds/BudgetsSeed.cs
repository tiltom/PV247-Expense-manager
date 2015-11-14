using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity.Budgets;

namespace ExpenseManager.Database.Seeding.Seeds
{
    internal class BudgetsSeed<TContext> : ISeeds<TContext>
        where TContext : DbContext, IBudgetContext
    {
        public void Seed(TContext context)
        {
            var budgets = new List<Budget>
            {
                new Budget
                {
                    Currency = context.Currencies.FirstOrDefault(),
                    StartDate = new DateTime(2015, 10, 15),
                    EndDate = new DateTime(2015, 10, 25),
                    Name = "Spain Holiday",
                    Description = "Budget for holiday in Spain",
                    Limit = 400,
                    Transactions = context.Transactions.Where(x => x.Description.Contains("Spain")).ToList(),
                    Creator = context.UserProfiles.FirstOrDefault(),
                    AccessRights = context.BudgetAccessRights.ToList()
                }
            };

            context.Budgets.AddRange(budgets);
            context.SaveChanges();
        }
    }
}