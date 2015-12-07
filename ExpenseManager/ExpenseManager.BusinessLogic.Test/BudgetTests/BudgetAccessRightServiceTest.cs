using System;
using ExpenseManager.BusinessLogic.BudgetServices;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Entity.Users;
using NUnit.Framework;

namespace ExpenseManager.BusinessLogic.Test.BudgetTests
{
    [TestFixture]
    internal class BudgetAccessRightServiceTest
    {
        [Test]
        [ExpectedException(typeof (ServiceValidationException))]
        public void ValidateBudgetBudgetAccessRight_NullBudget_ThrowException()
        {
            var budgetAccessRight = new BudgetAccessRight
            {
                Budget = null,
                Permission = PermissionEnum.Read,
                UserProfile = new UserProfile()
            };

            var budgetAccessRightService = new BudgetAccessRightService(ProvidersFactory.GetNewBudgetsProviders());
            budgetAccessRightService.Validate(budgetAccessRight);
        }

        [Test]
        [TestCase(null)]
        [ExpectedException(typeof (ArgumentNullException))]
        public void ValidateBudgetBudgetAccessRight_NullBudgetAccessRight_ThrowException(
            BudgetAccessRight budgetAccessRight)
        {
            var budgetAccessRightService = new BudgetAccessRightService(ProvidersFactory.GetNewBudgetsProviders());
            budgetAccessRightService.Validate(budgetAccessRight);
        }

        [Test]
        [ExpectedException(typeof (ServiceValidationException))]
        public void ValidateBudgetBudgetAccessRight_NullUserProfile_ThrowException()
        {
            var budgetAccessRight = new BudgetAccessRight
            {
                Budget = new Budget(),
                Permission = PermissionEnum.Read,
                UserProfile = null
            };

            var budgetAccessRightService = new BudgetAccessRightService(ProvidersFactory.GetNewBudgetsProviders());
            budgetAccessRightService.Validate(budgetAccessRight);
        }

        [Test]
        public void ValidateBudgetBudgetAccessRight_ValidBudgetAccessRight_ReturnsTrue()
        {
            var budgetAccessRight = new BudgetAccessRight
            {
                Budget = new Budget(),
                Permission = PermissionEnum.Read,
                UserProfile = new UserProfile()
            };

            var budgetAccessRightService = new BudgetAccessRightService(ProvidersFactory.GetNewBudgetsProviders());
            Assert.DoesNotThrow(() => budgetAccessRightService.Validate(budgetAccessRight));
        }
    }
}