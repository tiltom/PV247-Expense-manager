﻿using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Wallets;
using ExpenseManager.Web.DatabaseContexts;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ExpenseManager.Web.Models.User
{
    // You can add profile data for the user by adding more properties to your User class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

    public class ApplicationDbContext : IdentityDbContext<Entity.Users.User>
    {
        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            Database.SetInitializer(new ApplicationDbInitializer());
        }

        public ApplicationDbContext()
            : base("DefaultConnection", false)
        {
        }

        public DbSet<Entity.Budgets.BudgetAccessRight> BudgetAccessRights { get; set; }
        public DbSet<WalletAccessRight> WalletAccessRights { get; set; }
        public DbSet<Entity.Budgets.Budget> Budgets { get; set; }
        public DbSet<Entity.Wallets.Wallet> Wallets { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<RepeatableTransaction> RepeatableTransactions { get; set; }
        public DbSet<Entity.Transactions.Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}