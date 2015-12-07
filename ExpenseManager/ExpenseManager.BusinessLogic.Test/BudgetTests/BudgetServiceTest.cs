using System;
using ExpenseManager.BusinessLogic.BudgetServices;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Providers.Factory;
using NUnit.Framework;

namespace ExpenseManager.BusinessLogic.Test.BudgetTests
{
    [TestFixture]
    public class BudgetServiceTest
    {
        [Test]
        [ExpectedException(typeof (ServiceValidationException))]
        public void ValidateBudget_EmptyName_ThrowException()
        {
            var budget = new Budget
            {
                Name = string.Empty,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Limit = 10
            };

            var budgetService = new BudgetService(ProvidersFactory.GetNewBudgetsProviders(),
                ProvidersFactory.GetNewTransactionsProviders());
            budgetService.Validate(budget);
        }

        [Test]
        [ExpectedException(typeof (ServiceValidationException))]
        public void ValidateBudget_LimitLessThanZero_ThrowException()
        {
            var budget = new Budget
            {
                Name = "Test Name",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Limit = -10
            };

            var budgetService = new BudgetService(ProvidersFactory.GetNewBudgetsProviders(),
                ProvidersFactory.GetNewTransactionsProviders());
            budgetService.Validate(budget);
        }

        [Test]
        [TestCase(null)]
        [ExpectedException(typeof (ArgumentNullException))]
        public void ValidateBudget_NullBudget_ThrowException(Budget budget)
        {
            var budgetService = new BudgetService(ProvidersFactory.GetNewBudgetsProviders(),
                ProvidersFactory.GetNewTransactionsProviders());
            budgetService.Validate(budget);
        }

        [Test]
        public void ValidateBudget_ValidBudget_ReturnTrue()
        {
            var budget = new Budget
            {
                Name = "Test Name",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Limit = 10
            };

            var budgetService = new BudgetService(ProvidersFactory.GetNewBudgetsProviders(),
                ProvidersFactory.GetNewTransactionsProviders());
            Assert.DoesNotThrow(() => budgetService.Validate(budget));
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