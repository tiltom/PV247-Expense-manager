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
        public void ValidateBudgetBudgetAccessRight_NullBudget_ReturnsFalse()
        {
            var budgetAccessRight = new BudgetAccessRight
            {
                Budget = null,
                Permission = PermissionEnum.Read,
                UserProfile = new UserProfile()
            };

            var budgetAccessRightService = new BudgetAccessRightService(ProvidersFactory.GetNewBudgetsProviders());
            Assert.IsFalse(budgetAccessRightService.ValidateBudgetAccessRight(budgetAccessRight));
        }

        [Test]
        [TestCase(null)]
        public void ValidateBudgetBudgetAccessRight_NullBudgetAccessRight_ReturnFalse(
            BudgetAccessRight budgetAccessRight)
        {
            var budgetAccessRightService = new BudgetAccessRightService(ProvidersFactory.GetNewBudgetsProviders());
            Assert.IsFalse(budgetAccessRightService.ValidateBudgetAccessRight(budgetAccessRight));
        }

        [Test]
        public void ValidateBudgetBudgetAccessRight_NullUserProfile_ReturnsFalse()
        {
            var budgetAccessRight = new BudgetAccessRight
            {
                Budget = new Budget(),
                Permission = PermissionEnum.Read,
                UserProfile = null
            };

            var budgetAccessRightService = new BudgetAccessRightService(ProvidersFactory.GetNewBudgetsProviders());
            Assert.IsFalse(budgetAccessRightService.ValidateBudgetAccessRight(budgetAccessRight));
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
            Assert.IsTrue(budgetAccessRightService.ValidateBudgetAccessRight(budgetAccessRight));
        }
    }
}