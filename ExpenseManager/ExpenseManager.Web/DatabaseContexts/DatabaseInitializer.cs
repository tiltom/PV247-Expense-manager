﻿using System.Data.Entity;
using ExpenseManager.Web.Models.User;

namespace ExpenseManager.Web.DatabaseContexts
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            Database.SetInitializer(new ApplicationDbInitializer());
            var context = new ApplicationDbContext();
            context.Database.Initialize(true);
        }
    }
}