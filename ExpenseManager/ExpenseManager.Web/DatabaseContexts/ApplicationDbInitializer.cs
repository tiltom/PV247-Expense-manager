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
            InitializeWallets(context);
            InitializeTransactions(context);
            InitializeRepeatableTransactions(context);
            InitializeWalletAccessRights(context);

            InitializeBudgets(context);
            InitializeBudgetAccessRights(context);
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

        private static void InitializeBudgetAccessRights(ApplicationDbContext context)
        {
            var budgetAccessRights = new List<BudgetAccessRight>
            {
                new BudgetAccessRight
                {
                    Budget = context.Budgets.FirstOrDefault(),
                    Permission = PermissionEnum.Owner,
                    User = context.Users.FirstOrDefault()
                }
            };

            context.BudgetAccessRights.AddRange(budgetAccessRights);
        }

        private static void InitializeWalletAccessRights(ApplicationDbContext context)
        {
            var walletAccessRights = new List<WalletAccessRight>
            {
                new WalletAccessRight
                {
                    Wallet = context.Wallets.FirstOrDefault(),
                    Permission = PermissionEnum.Owner,
                    User = context.Users.FirstOrDefault()
                }
            };

            context.WalletAccessRights.AddRange(walletAccessRights);
        }

        private static void InitializeBudgets(ApplicationDbContext context)
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
                    Creator = context.Users.FirstOrDefault(),
                    AccessRights = context.BudgetAccessRights.ToList()
                }
            };

            context.Budgets.AddRange(budgets);
        }

        private static void InitializeWallets(ApplicationDbContext context)
        {
            var wallets = new List<Wallet>
            {
                new Wallet
                {
                    Currency = context.Currencies.FirstOrDefault(),
                    Name = "My Wallet"
                },
                new Wallet
                {
                    Currency = context.Currencies.FirstOrDefault(),
                    Name = "My wife's Wallet"
                },
                new Wallet
                {
                    Currency = context.Currencies.FirstOrDefault(),
                    Name = "My brother's Wallet"
                }
            };

            context.Wallets.AddRange(wallets);
        }

        private static void InitializeTransactions(ApplicationDbContext context)
        {
            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = 20,
                    Date = new DateTime(2015, 10, 17),
                    Category = context.Categories.FirstOrDefault(),
                    Description = "Found 20 euro on the ground"
                },
                new Transaction
                {
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = -10,
                    Date = new DateTime(2015, 10, 17),
                    Category = context.Categories.FirstOrDefault(),
                    Description = "Bought a ticket to the cinema"
                },
                new Transaction
                {
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = -5,
                    Date = new DateTime(2015, 10, 17),
                    Category = context.Categories.FirstOrDefault(),
                    Description = "Bet on a Chicago Blackhawks"
                },
                new Transaction
                {
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = -50,
                    Date = new DateTime(2015, 10, 16),
                    Category = context.Categories.Where(x => x.Description.Contains("transportation")).FirstOrDefault(),
                    Description = "Bought a ticket to Madrid"
                }
            };

            context.Transactions.AddRange(transactions);
        }

        private static void InitializeRepeatableTransactions(ApplicationDbContext context)
        {
            var repeatableTransactions = new List<RepeatableTransaction>
            {
                new RepeatableTransaction
                {
                    FirstTransaction = context.Transactions.Where(x => x.Description.Contains("Bet")).FirstOrDefault(),
                    Frequency = new TimeSpan(2, 0, 0),
                    LastOccurence = new DateTime(2015, 10, 17)
                }
            };

            context.RepeatableTransactions.AddRange(repeatableTransactions);
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