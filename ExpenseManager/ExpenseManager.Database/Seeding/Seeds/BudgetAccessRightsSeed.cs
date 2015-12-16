using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Budgets;

namespace ExpenseManager.Database.Seeding.Seeds
{
    internal class BudgetAccessRightsSeed<TContext> : ISeeds<TContext>
        where TContext : DbContext, IBudgetContext
    {
        public void Seed(TContext context)
        {
            var houseBudget = context.Budgets.Where(b => b.Description == "This year shared budget for our household").FirstOrDefault();
            var holidayBudget = context.Budgets.Where(b => b.Description == "Budget for holiday in Spain").FirstOrDefault();
            var betsBudget = context.Budgets.Where(b => b.Name == "Hazard").FirstOrDefault();
            var pepik = context.UserProfiles.Where(u => u.FirstName == "Pepik").FirstOrDefault();
            var kunhuta = context.UserProfiles.Where(u => u.FirstName == "Kunhuta").FirstOrDefault();

            var budgetAccessRights = new List<BudgetAccessRight>
            {

                new BudgetAccessRight
                {
                    Budget = houseBudget,
                    Permission = PermissionEnum.Owner,
                    UserProfile = pepik
                },

                new BudgetAccessRight
                {
                    Budget = holidayBudget,
                    Permission = PermissionEnum.Write,
                    UserProfile = pepik
                },

                new BudgetAccessRight
                {
                    Budget = holidayBudget,
                    Permission = PermissionEnum.Write,
                    UserProfile = kunhuta
                },

                new BudgetAccessRight
                {
                    Budget = holidayBudget,
                    Permission = PermissionEnum.Owner,
                    UserProfile = kunhuta
                },
                new BudgetAccessRight
                {
                    Budget = betsBudget,
                    Permission = PermissionEnum.Write,
                    UserProfile = kunhuta
                },

                new BudgetAccessRight
                {
                    Budget = betsBudget,
                    Permission = PermissionEnum.Owner,
                    UserProfile = pepik
                },
            };

            context.BudgetAccessRights.AddRange(budgetAccessRights);
            context.SaveChanges();
        }
    }
}