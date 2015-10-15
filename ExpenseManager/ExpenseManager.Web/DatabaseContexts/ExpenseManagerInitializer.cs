using System.Collections.Generic;
using System.Data.Entity;
using ExpenseManager.Entity;

namespace ExpenseManager.Web.DatabaseContexts
{
    public class ExpenseManagerInitializater :
        DropCreateDatabaseIfModelChanges<ExpenseManagerContext>
    {
        protected override void Seed(ExpenseManagerContext context)
        {
            InitializeCurrency(context);
            InitializeCategories(context);

            context.SaveChanges();
        }

        private static void InitializeCategories(ExpenseManagerContext context)
        {
            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Other",
                    Description = "Category for non-classifiable transactions",
                    IconPath = "glyphicons-circle-question-mark"
                },
                new Category
                {
                    Name = "Food & Drinks",
                    Description = "Category for comsumables",
                    IconPath = "glyphicons-fast-food"
                },
                new Category
                {
                    Name = "Travel",
                    Description = "Category for transportation and related stuff",
                    IconPath = "glyphicons-transport"
                }
            };
            context.Categories.AddRange(categories);
        }

        private static void InitializeCurrency(ExpenseManagerContext context)
        {
            var currencies = new List<Currency>
            {
                new Currency
                {
                    Name = "American Dollar",
                    Symbol = "$"
                },
                new Currency
                {
                    Name = "Česká koruna",
                    Symbol = "Kč"
                },
                new Currency
                {
                    Name = "Euro",
                    Symbol = "€"
                }
            };

            context.Currencies.AddRange(currencies);
        }
    }
}