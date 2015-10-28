using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;
using ExpenseManager.Web.Common;
using ExpenseManager.Web.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ExpenseManager.Web.DatabaseContexts
{
    public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeCurrency(context);
            InitializeCategories(context);
            InitializeIdentity(context);
            InitializeTransactions(context);
            InitializeRepeatableTransactions(context);
            InitializeBudgets(context);
            InitializeBudgetAccessRights(context);
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
                    Description = "Category for consumables",
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
            context.SaveChanges();
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
            context.SaveChanges();
        }

        private static void InitializeBudgetAccessRights(ApplicationDbContext context)
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
                    Creator = context.UserProfiles.FirstOrDefault(),
                    AccessRights = context.BudgetAccessRights.ToList()
                }
            };

            context.Budgets.AddRange(budgets);
            context.SaveChanges();
        }

        private static void InitializeTransactions(ApplicationDbContext context)
        {
            var wallet = context.Wallets.FirstOrDefault();

            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    Wallet = wallet,
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = 20,
                    Date = new DateTime(2015, 10, 17),
                    Category = context.Categories.FirstOrDefault(),
                    Description = "Found 20 euro on the ground"
                },
                new Transaction
                {
                    Wallet = wallet,
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = -10,
                    Date = new DateTime(2015, 10, 17),
                    Category = context.Categories.FirstOrDefault(),
                    Description = "Bought a ticket to the cinema"
                },
                new Transaction
                {
                    Wallet = wallet,
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = -5,
                    Date = new DateTime(2015, 10, 17),
                    Category = context.Categories.FirstOrDefault(),
                    Description = "Bet on a Chicago Blackhawks"
                },
                new Transaction
                {
                    Wallet = wallet,
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = -50,
                    Date = new DateTime(2015, 10, 16),
                    Category = context.Categories.FirstOrDefault(x => x.Description.Contains("transportation")),
                    Description = "Bought a ticket to Madrid"
                },
                new Transaction
                {
                    Wallet = context.Wallets.FirstOrDefault(u => u.Name.Contains("Read")),
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = -5,
                    Date = new DateTime(2015, 10, 17),
                    Category = context.Categories.FirstOrDefault(),
                    Description = "Read transaction"
                },
                new Transaction
                {
                    Wallet = context.Wallets.FirstOrDefault(u => u.Name.Contains("Write")),
                    Currency = context.Currencies.FirstOrDefault(),
                    Amount = -50,
                    Date = new DateTime(2015, 10, 16),
                    Category = context.Categories.FirstOrDefault(x => x.Description.Contains("transportation")),
                    Description = "Write transaction"
                }
            };

            context.Transactions.AddRange(transactions);
            context.SaveChanges();
        }

        private static void InitializeRepeatableTransactions(ApplicationDbContext context)
        {
            var firstTransaction = context.Transactions.FirstOrDefault();

            var repeatableTransactions = new List<RepeatableTransaction>
            {
                new RepeatableTransaction
                {
                    FirstTransaction = firstTransaction,
                    Frequency = new TimeSpan(2, 0, 0),
                    LastOccurence = new DateTime(2015, 10, 17)
                }
            };

            context.RepeatableTransactions.AddRange(repeatableTransactions);
            context.SaveChanges();
        }

        private static void InitializeIdentity(ApplicationDbContext context)
        {
            const string name = "admin@example.com";
            const string password = "password1";
            const string adminRoleName = "Admin";
            const string userRoleName = "User";
            const string firstName = "admin";
            const string lastName = "admin";

            const string nameUserRead = "userread@example.com";
            const string nameUserWrite = "userwrite@example.com";
            const string firstNameUserRead = "userRead";
            const string firstNameUserWrite = "userWrite";
            const string lastNameUser = "user";

            var userManager = new ApplicationUserManager(new UserStore<UserIdentity>(context));
            var roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context));

            //Create Role Admin if it does not exist
            CreateRole(roleManager, adminRoleName);
            CreateRole(roleManager, userRoleName);

            var user = userManager.FindByName(name);
            if (user == null)
            {
                var currency = context.Currencies.FirstOrDefault(c => c.Symbol == "Kč");
                var profile = new UserProfile
                {
                    PersonalWallet = new Wallet
                    {
                        Name = "Default Wallet",
                        Currency = currency
                    },
                    FirstName = firstName,
                    LastName = lastName
                };
                profile.WalletAccessRights = new List<WalletAccessRight>
                {
                    new WalletAccessRight
                    {
                        Permission = PermissionEnum.Owner,
                        UserProfile = profile,
                        Wallet = profile.PersonalWallet
                    }
                };
                profile = context.UserProfiles.Add(profile);
                context.SaveChanges();
                user = new UserIdentity
                {
                    UserName = name,
                    Email = name,
                    CreationDate = DateTime.Now,
                    Profile = profile
                };
                userManager.Create(user, password);
                userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add UserProfile admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            var role = roleManager.FindByName(adminRoleName);
            if (!rolesForUser.Contains(role.Name))
            {
                userManager.AddToRole(user.Id, role.Name);
            }
            context.SaveChanges();

            var adminProfile = context.UserProfiles.FirstOrDefault(u => u.FirstName == firstName);
            user = userManager.FindByName(nameUserRead);
            if (user == null)
            {
                var currency = context.Currencies.FirstOrDefault(c => c.Symbol == "Kč");
                var profile = new UserProfile
                {
                    PersonalWallet = new Wallet
                    {
                        Name = "Read Wallet",
                        Currency = currency
                    },
                    FirstName = firstNameUserRead,
                    LastName = lastNameUser
                };

                profile.WalletAccessRights = new List<WalletAccessRight>
                {
                    new WalletAccessRight
                    {
                        Permission = PermissionEnum.Owner,
                        UserProfile = profile,
                        Wallet = profile.PersonalWallet
                    } /* //What to change to make it work?
                    new WalletAccessRight
                    {
                        Permission = PermissionEnum.Read,
                        UserProfile = adminProfile,
                        Wallet = profile.PersonalWallet
                    }*/
                };
                profile = context.UserProfiles.Add(profile);
                context.SaveChanges();
                user = new UserIdentity
                {
                    UserName = nameUserRead,
                    Email = nameUserRead,
                    CreationDate = DateTime.Now,
                    Profile = profile
                };
                userManager.Create(user, password);
                userManager.SetLockoutEnabled(user.Id, false);
            }


            // Add UserProfile admin to Role User if not already added
            rolesForUser = userManager.GetRoles(user.Id);
            role = roleManager.FindByName(userRoleName);
            if (!rolesForUser.Contains(role.Name))
            {
                userManager.AddToRole(user.Id, role.Name);
            }
            context.SaveChanges();

            //TODO Remove
            var userReadAcess = new WalletAccessRight
            {
                Guid = Guid.NewGuid(),
                Permission = PermissionEnum.Read,
                UserProfile = context.UserProfiles.FirstOrDefault(u => u.FirstName == firstName),
                Wallet = context.Wallets.FirstOrDefault(w => w.Owner.FirstName == firstNameUserRead)
            };
            context.WalletAccessRights.Add(userReadAcess);
            context.SaveChanges();
            //TODO End Remove

            user = userManager.FindByName(nameUserWrite);
            if (user == null)
            {
                var currency = context.Currencies.FirstOrDefault(c => c.Symbol == "Kč");
                var profile = new UserProfile
                {
                    PersonalWallet = new Wallet
                    {
                        Name = "Write Wallet",
                        Currency = currency
                    },
                    FirstName = firstNameUserWrite,
                    LastName = lastNameUser
                };

                profile.WalletAccessRights = new List<WalletAccessRight>
                {
                    new WalletAccessRight
                    {
                        Permission = PermissionEnum.Owner,
                        UserProfile = profile,
                        Wallet = profile.PersonalWallet
                    } /* What to change to make it work?
                    new WalletAccessRight
                    {
                        Permission = PermissionEnum.Write,
                        UserProfile = adminProfile,
                        Wallet = profile.PersonalWallet
                    }*/
                };
                profile = context.UserProfiles.Add(profile);
                context.SaveChanges();
                user = new UserIdentity
                {
                    UserName = nameUserWrite,
                    Email = nameUserWrite,
                    CreationDate = DateTime.Now,
                    Profile = profile
                };
                userManager.Create(user, password);
                userManager.SetLockoutEnabled(user.Id, false);
            }


            // Add UserProfile admin to Role User if not already added
            rolesForUser = userManager.GetRoles(user.Id);
            role = roleManager.FindByName(userRoleName);
            if (!rolesForUser.Contains(role.Name))
            {
                userManager.AddToRole(user.Id, role.Name);
            }
            context.SaveChanges();

            //TODO Remove
            var userWriteAcess = new WalletAccessRight
            {
                Guid = Guid.NewGuid(),
                Permission = PermissionEnum.Write,
                UserProfile = context.UserProfiles.FirstOrDefault(u => u.FirstName == firstName),
                Wallet = context.Wallets.FirstOrDefault(w => w.Owner.FirstName == firstNameUserWrite)
            };
            context.WalletAccessRights.Add(userWriteAcess);
            context.SaveChanges();
            //TODO End Remove
        }


        private static void CreateRole(ApplicationRoleManager roleManager, string adminRoleName)
        {
            var role = roleManager.FindByName(adminRoleName);
            if (role != null) return;
            role = new IdentityRole(adminRoleName);
            roleManager.Create(role);
        }
    }
}