using System;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Factory;

namespace ExpenseManager.BusinessLogic
{
    public class CategoryService
    {
        private readonly ITransactionsProvider _db = ProvidersFactory.GetNewTransactionsProviders();

        public IQueryable GetCategories()
        {
            return this._db.Categories;
        }

        public async Task CreateCategory(Category category)
        {
            this.ValidateCategory(category);

            await this._db.AddOrUpdateAsync(category);
        }

        public Category GetCategoryByGuid(Guid guid)
        {
            return this._db.Categories.FirstOrDefault(x => x.Guid.Equals(guid));
        }

        public async Task EditCategory(Category category)
        {
            var categoryToEdit = this.GetCategoryByGuid(category.Guid);
            categoryToEdit.Name = category.Name;
            categoryToEdit.Description = category.Description;
            categoryToEdit.IconPath = category.IconPath;

            this.ValidateCategory(categoryToEdit);

            await this._db.AddOrUpdateAsync(categoryToEdit);
        }

        public async Task DeleteCategory(Guid guid)
        {
            var categoryToDelete = this.GetCategoryByGuid(guid);
            var defaultCategory = this.GetDefaultCategory();

            categoryToDelete.Transactions.ToList()
                .ForEach(t => t.Category = defaultCategory);

            await this._db.DeteleAsync(categoryToDelete);
        }

        #region private

        private Category GetDefaultCategory()
        {
            return this._db.Categories.FirstOrDefault();
        }

        private void ValidateCategory(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category must not be null");
            }

            if ("".Equals(category.Name))
            {
                throw new ArgumentException("Name must not be empty");
            }

            if ("".Equals(category.Description))
            {
                throw new ArgumentException("Description must not be empty");
            }
        }

        #endregion
    }
}