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
                    StartDate = DateTime.Now.AddDays(-5),
                    EndDate = DateTime.Now.AddDays(12),
                    Name = "Spain Holiday",
                    Description = "Budget for holiday in Spain",
                    Limit = 400
                },

                new Budget
                {
                    StartDate = DateTime.Now.AddDays(-100),
                    EndDate = DateTime.Now.AddDays(200),
                    Name = "Household",
                    Description = "This year shared budget for our household",
                    Limit = 20000
                },

                new Budget
                {
                    StartDate = DateTime.Now.AddDays(-150),
                    EndDate = DateTime.Now.AddDays(50),
                    Name = "Hazard",
                    Description = "Betsiky",
                    Limit = 10000
                }
            };

            context.Budgets.AddRange(budgets);
            context.SaveChanges();
        }
    }
}