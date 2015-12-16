using System;
using System.Data.Entity;
using System.Linq;
using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ExpenseManager.Database.Seeding.Seeds
{
    internal class UsersSeed<TContext> : ISeeds<TContext>
        where TContext : DbContext, IUserContext
    {
        private const string AdminEmail = "admin@example.com";


        private readonly RegisterUserInfo _adminUserInfo = new RegisterUserInfo
        {
            FirstName = "admin",
            LastName = "admin",
            Email = "admin@example.com",
            WalletName = "Default Wallet",
            RoleName = UserIdentity.AdminRole,
            Password = "password1"
        };

        private readonly RegisterUserInfo _readUserInfo = new RegisterUserInfo
        {
            FirstName = "Pepik",
            LastName = "Okurka",
            Email = "pepik.vokurkacek007@seznam.com",
            WalletName = "Pepik Wallet",
            RoleName = UserIdentity.UserRole,
            Password = "password1"
        };

        private readonly RegisterUserInfo _writeUserInfo = new RegisterUserInfo
        {
            FirstName = "Kunhuta",
            LastName = "Uherská",
            Email = "kunhuticka.uhrickohalicska@seznam.cz",
            WalletName = "Kunhuta Wallet",
            RoleName = UserIdentity.UserRole,
            Password = "password1"
        };


        public void Seed(TContext context)
        {
            var userManager = new UserManager<UserIdentity>(new UserStore<UserIdentity>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            //Create Role Admin if it does not exist
            CreateRole(roleManager, UserIdentity.AdminRole);
            CreateRole(roleManager, UserIdentity.UserRole);

            CreateUser(context, userManager, roleManager, this._adminUserInfo);
            CreateUserWithPermission(context, userManager, roleManager, this._readUserInfo, PermissionEnum.Read);
            CreateUserWithPermission(context, userManager, roleManager, this._writeUserInfo, PermissionEnum.Write);
        }

        private static void CreateUserWithPermission(TContext context, UserManager<UserIdentity, string> userManager,
            RoleManager<IdentityRole, string> roleManager, RegisterUserInfo userInfo, PermissionEnum permission)
        {
            CreateUser(context, userManager, roleManager, userInfo);

            var admin = userManager.FindByEmail(AdminEmail);
            var adminUserProfile = context.UserProfiles.FirstOrDefault(u => u.Guid == admin.Profile.Guid);

            var user = userManager.FindByEmail(userInfo.Email);
            var wallet =
                context.WalletAccessRights.FirstOrDefault(
                    war => war.UserProfile.Guid == user.Profile.Guid
                           && war.Permission == PermissionEnum.Owner
                    )?.Wallet;

            CreateWalletAccessRight(context, adminUserProfile, wallet, permission);
        }

        private static void CreateUser(TContext context, UserManager<UserIdentity, string> userManager,
            RoleManager<IdentityRole, string> roleManager, RegisterUserInfo userInfo)
        {
            var currency = context.Currencies.FirstOrDefault(c => c.Symbol == "Kč");
            var profile = CreateProfile(context, userInfo.FirstName, userInfo.LastName);
            var wallet = CreateWallet(context, currency, userInfo.WalletName);
            CreateWalletAccessRight(context, profile, wallet, PermissionEnum.Owner);

            var user = CreateUserIdentity(userManager, profile, userInfo.Email, userInfo.Password);
            RegisterUserToRole(context, userManager, roleManager, user, userInfo.RoleName);
        }

        private static void RegisterUserToRole(TContext context, UserManager<UserIdentity, string> userManager,
            RoleManager<IdentityRole, string> roleManager, UserIdentity user, string roleName)
        {
            var rolesForUser = userManager.GetRoles(user.Id);
            var role = roleManager.FindByName(roleName);
            if (!rolesForUser.Contains(role.Name))
            {
                userManager.AddToRole(user.Id, role.Name);
            }
            context.SaveChanges();
        }

        private static UserIdentity CreateUserIdentity(UserManager<UserIdentity, string> userManager,
            UserProfile profile, string email, string password)
        {
            var user = new UserIdentity
            {
                UserName = email,
                Email = email,
                CreationDate = DateTime.Now,
                Profile = profile
            };
            userManager.Create(user, password);
            userManager.SetLockoutEnabled(user.Id, false);
            return user;
        }

        private static void CreateWalletAccessRight(TContext context, UserProfile profile, Wallet wallet,
            PermissionEnum permission)
        {
            var personalWalletAccessRight = new WalletAccessRight
            {
                Permission = permission,
                UserProfile = profile,
                Wallet = wallet
            };
            context.WalletAccessRights.Add(personalWalletAccessRight);
            context.SaveChanges();
        }

        private static Wallet CreateWallet(TContext context, Currency currency, string walletName)
        {
            var wallet = new Wallet
            {
                Name = walletName,
                Currency = currency
            };
            wallet = context.Wallets.Add(wallet);
            context.SaveChanges();
            return wallet;
        }

        private static UserProfile CreateProfile(TContext context, string firstName, string lastName)
        {
            var profile = new UserProfile
            {
                FirstName = firstName,
                LastName = lastName
            };
            profile = context.UserProfiles.Add(profile);
            context.SaveChanges();
            return profile;
        }

        private static void CreateRole(RoleManager<IdentityRole, string> roleManager, string roleName)
        {
            var role = roleManager.FindByName(roleName);
            if (role != null) return;
            role = new IdentityRole(roleName);
            roleManager.Create(role);
        }

        private class RegisterUserInfo
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string WalletName { get; set; }
            public string RoleName { get; set; }
        }
    }
}