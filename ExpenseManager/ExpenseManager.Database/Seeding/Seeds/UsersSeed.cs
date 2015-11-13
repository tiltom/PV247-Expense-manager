using ExpenseManager.Database.Common;
using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.Database.Seeding.Seeds
{
    internal class UsersSeed<TContext> : ISeeds<TContext>
        where TContext : DbContext, IUserContext
    {
        public void Seed(TContext context)
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

            var userManager = new UserManager<UserIdentity>(new UserStore<UserIdentity>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            //Create Role Admin if it does not exist
            CreateRole(roleManager, adminRoleName);
            CreateRole(roleManager, userRoleName);

            var user = userManager.FindByName(name);
            if (user == null)
            {
                var currency = context.Currencies.FirstOrDefault(c => c.Symbol == "Kč");

                

                var profile = new UserProfile
                {
                    FirstName = firstName,
                    LastName = lastName
                };
                profile = context.UserProfiles.Add(profile);
                context.SaveChanges();

                var wallet = new Wallet
                {
                    Name = "Default Wallet",
                    Currency = currency,
                    Owner = profile
                };
                wallet = context.Wallets.Add(wallet);
                context.SaveChanges();

                var personalWalletAccessRight = new WalletAccessRight
                {
                    Permission = PermissionEnum.Owner,
                    UserProfile = profile,
                    Wallet = wallet
                };
                var pwar = context.WalletAccessRights.Add(personalWalletAccessRight);
                context.SaveChanges();

                //profile.WalletAccessRights = new List<WalletAccessRight>
                //{
                //    pwar
                //};
                //profile = context.UserProfiles.Add(profile);
                //context.SaveChanges();
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
        private static void CreateRole(RoleManager<IdentityRole> roleManager, string adminRoleName)
        {
            var role = roleManager.FindByName(adminRoleName);
            if (role != null) return;
            role = new IdentityRole(adminRoleName);
            roleManager.Create(role);
        }
    }
}
