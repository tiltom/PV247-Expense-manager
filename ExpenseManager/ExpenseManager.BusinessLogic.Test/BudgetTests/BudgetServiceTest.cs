using System;
using ExpenseManager.BusinessLogic.BudgetServices;
using NUnit.Framework;

namespace ExpenseManager.BusinessLogic.Test.BudgetTests
{
    [TestFixture]
    public class BudgetServiceTest
    {
        [Test]
        [TestCase("12/03/2015", "10/03/2015")]
        public void ValidateModel_InvalidDates_ReturnFalse(DateTime startDate, DateTime endDate)
        {
            Assert.IsFalse(BudgetService.ValidateModel(startDate, endDate));
        }

        [Test]
        [TestCase("09/03/2015", "10/03/2015")]
        public void ValidateModel_ValidDates_ReturnTrue(DateTime startDate, DateTime endDate)
        {
            Assert.IsTrue(BudgetService.ValidateModel(startDate, endDate));
        }
    }
}