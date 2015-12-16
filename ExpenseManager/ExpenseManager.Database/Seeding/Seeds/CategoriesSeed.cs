using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Enums;

namespace ExpenseManager.Database.Seeding.Seeds
{
    internal class CategoriesSeed<TContext> : ISeeds<TContext>
        where TContext : DbContext, ITransactionContext, IWalletContext
    {
        public void Seed(TContext context)
        {
            var user = context.UserProfiles.FirstOrDefault(x => x.FirstName == "admin");

            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Other",
                    Description = "Category for non-classifiable transactions",
                    IconPath = "glyphicon-question-sign",
                    Type = CategoryType.Expense,
                    User = user
                },
                new Category
                {
                    Name = "Food & Drinks",
                    Description = "Category for consumables",
                    IconPath = "glyphicon-glass",
                    Type = CategoryType.Expense,
                    User = user
                },
                new Category
                {
                    Name = "Travel",
                    Description = "Category for transportation and related stuff",
                    IconPath = "glyphicon-plane",
                    Type = CategoryType.Expense,
                    User = user
                },
                new Category
                {
                    Name = "Salary",
                    Description = "Category for income from working",
                    IconPath = "glyphicon-usd",
                    Type = CategoryType.Income,
                    User = user
                },
                new Category
                {
                    Name = "Bitcoin",
                    Description = "Category for investments in Bitcoin",
                    IconPath = "glyphicon-bitcoin",
                    Type = CategoryType.IncomeAndExpense,
                    User = user
                }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();
        }
    }
}