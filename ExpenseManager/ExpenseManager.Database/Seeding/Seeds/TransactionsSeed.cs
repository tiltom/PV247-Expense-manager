using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExpenseManager.Database.Contexts;
using ExpenseManager.Entity.Transactions;

namespace ExpenseManager.Database.Seeding.Seeds
{
    internal class TransactionsSeed<TContext> : ISeeds<TContext>
        where TContext : DbContext, ITransactionContext
    {
        public void Seed(TContext context)
        {
            var kunhutaWallet = context.Wallets.Where(kw => kw.Name == "Kunhuta Wallet").FirstOrDefault();
            var pepikWallet = context.Wallets.Where(pw => pw.Name == "Pepik Wallet").FirstOrDefault();

            var foodCategory = context.Categories.Where(fc => fc.Name == "Food & Drinks").FirstOrDefault();
            var travelCategory = context.Categories.Where(tc => tc.Name == "Travel").FirstOrDefault();
            var salaryCategory = context.Categories.Where(tc => tc.Name == "Salary").FirstOrDefault();
            var carCategory = context.Categories.Where(cc => cc.Name == "Car").FirstOrDefault();
            var otherCategory = context.Categories.Where(oc => oc.Name == "Other").FirstOrDefault();
            var betsCategory = context.Categories.Where(bc => bc.Name == "Bets").FirstOrDefault();
            var houseCategory = context.Categories.Where(hc => hc.Name == "Household").FirstOrDefault();
            var drugsCategory = context.Categories.Where(hc => hc.Name == "Drugs").FirstOrDefault();

            var houseBudget = context.Budgets.Where(b => b.Description == "This year shared budget for our household").FirstOrDefault();
            var holidayBudget = context.Budgets.Where(b => b.Description == "Budget for holiday in Spain").FirstOrDefault();
            var betsBudget = context.Budgets.Where(b => b.Name == "Hazard").FirstOrDefault();

            var currency = context.Currencies.Where(c => c.Code == "CZK").FirstOrDefault();

            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    Wallet = kunhutaWallet,
                    Currency = currency,
                    Amount = -230,
                    Date = DateTime.Now.AddDays(-80),
                    Category = otherCategory,
                    Description = "Bought a ticket to the cinema"
                },
                new Transaction
                {
                    Wallet = kunhutaWallet,
                    Currency = currency,
                    Amount = 2000,
                    Date = DateTime.Now.AddDays(-50),
                    Category = salaryCategory,
                    Description = "Found 2000 euro on the ground"
                },
                new Transaction
                {
                    Wallet = pepikWallet,
                    Currency = currency,
                    Amount = -9000,
                    Date = DateTime.Now.AddDays(-40),
                    Category = houseCategory,
                    Description = "Rent",
                    Budget = houseBudget
                },
                new Transaction
                {
                    Wallet = pepikWallet,
                    Currency = currency,
                    Amount = 25000,
                    Date = DateTime.Now.AddDays(-30),
                    Category = salaryCategory,
                    Description = "Scuba Diver Job"
                },
                new Transaction
                {
                    Wallet = pepikWallet,
                    Currency = currency,
                    Amount = 20,
                    Date = DateTime.Now.AddDays(-30),
                    Category = salaryCategory,
                    Description = "Found 20 euro on the ground"
                },
                new Transaction
                {
                    Wallet = pepikWallet,
                    Currency = currency,
                    Amount = -3300,
                    Date = DateTime.Now.AddDays(-25),
                    Category = betsCategory,
                    Description = "Slot machine",
                    Budget = betsBudget
                },
                new Transaction
                {
                    Wallet = kunhutaWallet,
                    Currency = currency,
                    Amount = 35000,
                    Date = DateTime.Now.AddDays(-25),
                    Category = context.Categories.FirstOrDefault(x => x.Name.Contains("Salary")),
                    Description = "Money from the king"
                },
                new Transaction
                {
                    Wallet = pepikWallet,
                    Currency = currency,
                    Amount = 300,
                    Date = DateTime.Now.AddDays(-15),
                    Category = betsCategory,
                    Description = "Slot machine",
                    Budget = betsBudget
                },
                new Transaction
                {
                    Wallet = pepikWallet,
                    Currency = currency,
                    Amount = 20000,
                    Date = DateTime.Now.AddDays(-15),
                    Category = drugsCategory,
                    Description = "Cannabis"
                },
                new Transaction
                {
                    Wallet = pepikWallet,
                    Currency = currency,
                    Amount = -340,
                    Date = DateTime.Now.AddDays(-12),
                    Category = foodCategory,
                    Description = "Dinner at restaurant"
                },
                new Transaction
                {
                    Wallet = kunhutaWallet,
                    Currency = currency,
                    Amount = -500,
                    Date = DateTime.Now.AddDays(-11),
                    Category = betsCategory,
                    Description = "Bet on a Chicago Blackhawks"
                },
                new Transaction
                {
                    Wallet = kunhutaWallet,
                    Currency = currency,
                    Amount = 5000,
                    Date = DateTime.Now.AddDays(-11),
                    Category = betsCategory,
                    Description = "Win from Chicago Blackhawks bet"
                },
                new Transaction
                {
                    Wallet = kunhutaWallet,
                    Currency = currency,
                    Amount = -1500,
                    Date = DateTime.Now.AddDays(-11),
                    Category = betsCategory,
                    Description = "Bet on a Kometa Brno",
                    Budget = betsBudget
                },
                new Transaction
                {
                    Wallet = kunhutaWallet,
                    Currency = currency,
                    Amount = 5500,
                    Date = DateTime.Now.AddDays(-11),
                    Category = betsCategory,
                    Description = "Win from Kometa Brno bet (yeah they lost again, how typical.)",
                    Budget = betsBudget
                },
                new Transaction
                {
                    Wallet = kunhutaWallet,
                    Currency = currency,
                    Amount = -250,
                    Date = DateTime.Now.AddDays(-8),
                    Category = otherCategory,
                    Description = "Bought a ticket to the cinema"
                },
                new Transaction
                {
                    Wallet = pepikWallet,
                    Currency = currency,
                    Amount = 20000,
                    Date = DateTime.Now.AddDays(-7),
                    Category = drugsCategory,
                    Description = "Cannabis"
                },
                new Transaction
                {
                    Wallet = pepikWallet,
                    Currency = currency,
                    Amount = -5000,
                    Date = DateTime.Now.AddDays(-7),
                    Category = drugsCategory,
                    Description = "Lights"
                },
                new Transaction
                {
                    Wallet = pepikWallet,
                    Currency = currency,
                    Amount = -300,
                    Date = DateTime.Now.AddDays(-7),
                    Category = drugsCategory,
                    Description = "Seeds"
                },
                new Transaction
                {
                    Wallet = pepikWallet,
                    Currency = currency,
                    Amount = 120000,
                    Date = DateTime.Now.AddDays(-7),
                    Category = salaryCategory,
                    Description = "Asp.net expense manager payment"
                },
                new Transaction
                {
                    Wallet = pepikWallet,
                    Currency = currency,
                    Amount = -60000,
                    Date = DateTime.Now.AddDays(-7),
                    Category = carCategory,
                    Description = "mazda 6"
                },
                new Transaction
                {
                    Wallet = pepikWallet,
                    Currency = currency,
                    Amount = -310,
                    Date = DateTime.Now.AddDays(-7),
                    Category = foodCategory,
                    Description = "Dinner at restaurant"
                },
                new Transaction
                {
                    Wallet = pepikWallet,
                    Currency = currency,
                    Amount = -250,
                    Date = DateTime.Now.AddDays(-6),
                    Category = foodCategory,
                    Description = "Dinner at restaurant"
                },
                new Transaction
                {
                    Wallet = pepikWallet,
                    Currency = currency,
                    Amount = -4300,
                    Date = DateTime.Now.AddDays(-5),
                    Category = betsCategory,
                    Description = "Slot machine",
                    Budget = betsBudget
                },
                new Transaction
                {
                    Wallet = pepikWallet,
                    Currency = currency,
                    Amount = -390,
                    Date = DateTime.Now.AddDays(-3),
                    Category = foodCategory,
                    Description = "Dinner at restaurant"
                },
                new Transaction
                {
                    Wallet = kunhutaWallet,
                    Currency = currency,
                    Amount = -5000,
                    Date = DateTime.Now.AddDays(-2),
                    Category = travelCategory,
                    Description = "Bought a ticket to Madrid",
                    Budget = holidayBudget
                },
                new Transaction
                {
                    Wallet = pepikWallet,
                    Currency = currency,
                    Amount = 150,
                    Date = DateTime.Now.AddDays(-1),
                    Category = betsCategory,
                    Description = "Slot machine",
                    Budget = betsBudget
                },
            };

            context.Transactions.AddRange(transactions);
            context.SaveChanges();
        }
    }
}