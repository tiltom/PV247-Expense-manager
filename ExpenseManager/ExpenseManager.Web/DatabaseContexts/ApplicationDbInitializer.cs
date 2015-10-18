using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExpenseManager.Entity;
using ExpenseManager.Web.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ExpenseManager.Web.DatabaseContexts
{
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeCurrency(context);
            InitializeCategories(context);
            InitializeIdentityForEf(context);
            base.Seed(context);
            context.SaveChanges();
        }

        private static void InitializeCategories(ApplicationDbContext context)
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

        private static void InitializeCurrency(ApplicationDbContext context)
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

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
        private static void InitializeIdentityForEf(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<User>(context));
            var roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context));
            const string name = "admin@example.com";
            const string password = "password1";
            const string adminRoleName = "Admin";
            const string userRoleName = "User";


            //Create Role Admin if it does not exist
            CreateRole(roleManager, adminRoleName);
            CreateRole(roleManager, userRoleName);

            var user = userManager.FindByName(name);
            if (user == null)
            {
                var currency = context.Currencies.FirstOrDefault(c => c.Symbol == "Kč");
                user = new User
                {
                    UserName = name,
                    Email = name,
                    CreationDate = DateTime.Now,
                    PersonalWallet = new Wallet
                    {
                        Name = "Default Wallet",
                        Currency = currency
                    }
                };
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            var role = roleManager.FindByName(adminRoleName);
            if (!rolesForUser.Contains(role.Name))
            {
                var result = userManager.AddToRole(user.Id, role.Name);
            }
        }

        private static void CreateRole(ApplicationRoleManager roleManager, string adminRoleName)
        {
            var role = roleManager.FindByName(adminRoleName);
            if (role != null) return;
            role = new IdentityRole(adminRoleName);
            var roleresult = roleManager.Create(role);
        }
    }
}