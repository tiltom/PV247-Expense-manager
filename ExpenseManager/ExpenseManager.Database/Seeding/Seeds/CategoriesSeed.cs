using System.Collections.Generic;
using System.Data.Entity;
using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Enums;

namespace ExpenseManager.Database.Seeding.Seeds
{
    internal class CategoriesSeed<TContext> : ISeeds<TContext>
        where TContext : DbContext, ITransactionContext
    {
        public void Seed(TContext context)
        {
            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Other",
                    Description = "Category for non-classifiable transactions",
                    IconPath = "glyphicons-circle-question-mark",
                    Type = CategoryType.Expense
                },
                new Category
                {
                    Name = "Food & Drinks",
                    Description = "Category for consumables",
                    IconPath = "glyphicons-fast-food",
                    Type = CategoryType.Expense
                },
                new Category
                {
                    Name = "Travel",
                    Description = "Category for transportation and related stuff",
                    IconPath = "glyphicons-transport",
                    Type = CategoryType.Expense
                },
                new Category
                {
                    Name = "Salary",
                    Description = "Category for income from working",
                    IconPath = "glyphicons-money",
                    Type = CategoryType.Income
                },
                new Category
                {
                    Name = "Bitcoin",
                    Description = "Category for investments in Bitcoin",
                    IconPath = "glyphicons-bitcoin",
                    Type = CategoryType.IncomeAndExpense
                }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();
        }
    }
}