using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Budgets;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.Database.Seeding.Seeds
{
    internal class BudgetAccessRightsSeed<TContext> : ISeeds<TContext>
        where TContext : DbContext, IBudgetContext
    {
        public void Seed(TContext context)
        {
            var budget = context.Budgets.FirstOrDefault();

            var budgetAccessRights = new List<BudgetAccessRight>
            {
                new BudgetAccessRight
                {
                    Budget = budget,
                    Permission = PermissionEnum.Owner,
                    UserProfile = context.UserProfiles.FirstOrDefault()
                }
            };

            context.BudgetAccessRights.AddRange(budgetAccessRights);
            context.SaveChanges();
        }
    }
}
