using System;
using System.Collections.Generic;
using System.Data.Entity;
using ExpenseManager.Entity;

namespace ExpenseManager.BusinessLogic.DatabaseContext
{
    public class ExpenseManagerInitializater : DropCreateDatabaseAlways<ExpenseManagerContext>
    {
        protected override void Seed(ExpenseManagerContext context)
        {
            InitializeUsers(context);
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
                    Icon = "glyphicons-circle-question-mark"
                },
                new Category
                {
                    Name = "Food & Drinks",
                    Description = "Category for comsumables",
                    Icon = "glyphicons-fast-food"
                },
                new Category
                {
                    Name = "Travel",
                    Description = "Category for transportation and related stuff",
                    Icon = "glyphicons-transport"
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

        private static void InitializeUsers(ExpenseManagerContext context)
        {
            var users = new List<User>
            {
                new User
                {
                    //AccessRights = null,
                    CreateTime = DateTime.Now,
                    Email = "test@test.com",
                    Password = "test",
                    UserName = "test"
                    //Wallets = null
                }
            };

            context.Users.AddRange(users);
        }
    }
}