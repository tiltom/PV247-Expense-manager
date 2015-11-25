using System;
using ExpenseManager.BusinessLogic.BudgetServices;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Entity.Users;
using NUnit.Framework;

namespace ExpenseManager.BusinessLogic.Test.BudgetTests
{
    [TestFixture]
    public class BudgetServiceTest
    {
        [Test]
        public void ValidateBudget_EmptyName_ReturnFalse()
        {
            var budget = new Budget
            {
                Creator = new UserProfile(),
                Currency = new Currency(),
                Name = string.Empty,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Limit = 10
            };

            var budgetService = new BudgetService(ProvidersFactory.GetNewBudgetsProviders(),
                ProvidersFactory.GetNewTransactionsProviders());
            Assert.IsFalse(budgetService.ValidateBudget(budget));
        }

        [Test]
        public void ValidateBudget_LimitLessThanZero_ReturnFalse()
        {
            var budget = new Budget
            {
                Creator = new UserProfile(),
                Currency = new Currency(),
                Name = "Test Name",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Limit = -10
            };

            var budgetService = new BudgetService(ProvidersFactory.GetNewBudgetsProviders(),
                ProvidersFactory.GetNewTransactionsProviders());
            Assert.IsFalse(budgetService.ValidateBudget(budget));
        }

        [Test]
        [TestCase(null)]
        public void ValidateBudget_NullBudget_ReturnFalse(Budget budget)
        {
            var budgetService = new BudgetService(ProvidersFactory.GetNewBudgetsProviders(),
                ProvidersFactory.GetNewTransactionsProviders());
            Assert.IsFalse(budgetService.ValidateBudget(budget));
        }

        [Test]
        public void ValidateBudget_NullCreator_ReturnFalse()
        {
            var budget = new Budget
            {
                Creator = null,
                Currency = new Currency(),
                Name = "Test name",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Limit = 10
            };

            var budgetService = new BudgetService(ProvidersFactory.GetNewBudgetsProviders(),
                ProvidersFactory.GetNewTransactionsProviders());
            Assert.IsFalse(budgetService.ValidateBudget(budget));
        }

        [Test]
        public void ValidateBudget_NullCurrency_ReturnFalse()
        {
            var budget = new Budget
            {
                Creator = new UserProfile(),
                Currency = null,
                Name = "Test name",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Limit = 10
            };

            var budgetService = new BudgetService(ProvidersFactory.GetNewBudgetsProviders(),
                ProvidersFactory.GetNewTransactionsProviders());
            Assert.IsFalse(budgetService.ValidateBudget(budget));
        }

        [Test]
        public void ValidateBudget_ValidBudget_ReturnTrue()
        {
            var budget = new Budget
            {
                Creator = new UserProfile(),
                Currency = new Currency(),
                Name = "Test Name",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Limit = 10
            };

            var budgetService = new BudgetService(ProvidersFactory.GetNewBudgetsProviders(),
                ProvidersFactory.GetNewTransactionsProviders());
            Assert.IsTrue(budgetService.ValidateBudget(budget));
        }

        [Test]
        [TestCase("12/03/2015", "10/03/2015")]
        public void ValidateModel_InvalidDates_ReturnFalse(DateTime startDate, DateTime endDate)
        {
            Assert.IsFalse(BudgetService.ValidateModel(startDate, endDate));
        }

        [Test]
        [TestCase("09/03/2015", "09/03/2015")]
        public void ValidateModel_SameDates_ReturnTrue(DateTime startDate, DateTime endDate)
        {
            Assert.IsTrue(BudgetService.ValidateModel(startDate, endDate));
        }

        [Test]
        [TestCase("09/03/2015", "10/03/2015")]
        public void ValidateModel_ValidDates_ReturnTrue(DateTime startDate, DateTime endDate)
        {
            Assert.IsTrue(BudgetService.ValidateModel(startDate, endDate));
        }
    }
}