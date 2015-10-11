using System;
using System.Collections.Generic;
using System.Data.Entity;
using ExpenseManager.Entity;

namespace ExpenseManager.Web.DatabaseContext
{
    public class ExpenseManagerInitializater : DropCreateDatabaseIfModelChanges<ExpenseManagerContext>
    {
        protected override void Seed(ExpenseManagerContext context)
        {
            var users = new List<User>
            {
                new User
                {
                    AccessRights = null,
                    CreateTime = DateTime.Now,
                    Email = "test@test.com",
                    Password = "test",
                    UserName = "test",
                    Wallets = null
                }
            };

            users.ForEach(s => context.Users.Add(s));
            context.SaveChanges();
        }
    }
}