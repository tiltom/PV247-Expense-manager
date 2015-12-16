using System;
using ExpenseManager.BusinessLogic.CategoryServices;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Enums;
using ExpenseManager.Entity.Providers.Factory;
using NUnit.Framework;

namespace ExpenseManager.BusinessLogic.Test.CategoryTests
{
    [TestFixture]
    internal class CategoryServiceTest
    {
        [Test]
        [ExpectedException(typeof (ServiceValidationException))]
        public void ValidateCategory_EmptyDescription_ThrowException()
        {
            var category = new Category
            {
                Name = "Test name",
                Description = string.Empty,
                IconPath = "glyphicon-record",
                Type = CategoryType.Expense
            };

            var categoryService = new CategoryService(ProvidersFactory.GetNewTransactionsProviders());
            categoryService.Validate(category);
        }

        [Test]
        [ExpectedException(typeof (ServiceValidationException))]
        public void ValidateCategory_EmptyName_ThrowException()
        {
            var category = new Category
            {
                Name = string.Empty,
                Description = "Test description",
                IconPath = "glyphicon-record",
                Type = CategoryType.Expense
            };

            var categoryService = new CategoryService(ProvidersFactory.GetNewTransactionsProviders());
            categoryService.Validate(category);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void ValidateCategory_NullCategory_ThrowException()
        {
            var categoryService = new CategoryService(ProvidersFactory.GetNewTransactionsProviders());
            categoryService.Validate(null);
        }

        [Test]
        public void ValidateCategory_ValidCategory_ReturnTrue()
        {
            var category = new Category
            {
                Name = "Test name",
                Description = "Test description",
                IconPath = "glyphicon-record",
                Type = CategoryType.Expense
            };

            var categoryService = new CategoryService(ProvidersFactory.GetNewTransactionsProviders());
            Assert.DoesNotThrow(() => categoryService.Validate(category));
        }
    }
}