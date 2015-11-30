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
        public void ValidateCategory_EmptyDescription_ReturnFalse()
        {
            var category = new Category
            {
                Name = "Test name",
                Description = string.Empty,
                IconPath = "glyphicon-record",
                Type = CategoryType.Expense
            };

            var categoryService = new CategoryService(ProvidersFactory.GetNewTransactionsProviders());
            Assert.IsFalse(categoryService.ValidateCategory(category));
        }

        [Test]
        public void ValidateCategory_EmptyName_ReturnFalse()
        {
            var category = new Category
            {
                Name = string.Empty,
                Description = "Test description",
                IconPath = "glyphicon-record",
                Type = CategoryType.Expense
            };

            var categoryService = new CategoryService(ProvidersFactory.GetNewTransactionsProviders());
            Assert.IsFalse(categoryService.ValidateCategory(category));
        }

        [Test]
        public void ValidateCategory_NotSupportedGlyphicon_ReturnFalse()
        {
            var category = new Category
            {
                Name = "Test name",
                Description = "Test description",
                IconPath = "Not supported",
                Type = CategoryType.Expense
            };

            var categoryService = new CategoryService(ProvidersFactory.GetNewTransactionsProviders());
            Assert.IsFalse(categoryService.ValidateCategory(category));
        }

        [Test]
        public void ValidateCategory_NullCategory_ReturnFalse()
        {
            var categoryService = new CategoryService(ProvidersFactory.GetNewTransactionsProviders());
            Assert.IsFalse(categoryService.ValidateCategory(null));
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
            Assert.IsTrue(categoryService.ValidateCategory(category));
        }
    }
}