using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.BusinessLogic.Validators;
using ExpenseManager.BusinessLogic.Validators.Extensions;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Providers;

namespace ExpenseManager.BusinessLogic.CategoryServices
{
    /// <summary>
    ///     Class that handles logic of CategoryController
    /// </summary>
    public class CategoryService : IServiceValidation<Category>
    {
        private readonly ITransactionsProvider _db;
        private readonly CategoryValidator _validator;

        public CategoryService(ITransactionsProvider db)
        {
            this._db = db;
            this._validator = new CategoryValidator();
        }

        /// <summary>
        ///     Validates category
        /// </summary>
        /// <param name="category">Category to validate</param>
        /// <returns>True if category is valid, false otherwise</returns>
        public void Validate(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            this._validator.ValidateAndThrowCustomException(category);
        }

        /// <summary>
        ///     Returns all categories from database
        /// </summary>
        /// <returns>All categories</returns>
        public IQueryable GetCategories()
        {
            return this._db.Categories;
        }

        /// <summary>
        ///     Creates new category - adds it to database
        /// </summary>
        /// <param name="category">New category</param>
        /// <returns></returns>
        public async Task CreateCategory(Category category)
        {
            this.Validate(category);
            await this._db.AddOrUpdateAsync(category);
        }

        /// <summary>
        ///     Returns specified category by guid
        /// </summary>
        /// <param name="guid">ID that specifies returned category</param>
        /// <returns>Desired category</returns>
        public async Task<Category> GetCategoryByGuid(Guid guid)
        {
            return await this._db.Categories.Where(x => x.Guid.Equals(guid)).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Edits category and saves it to database
        /// </summary>
        /// <param name="category">Edited category</param>
        /// <returns></returns>
        public async Task EditCategory(Category category)
        {
            this.Validate(category);
            var categoryToEdit = await this.GetCategoryByGuid(category.Guid);
            categoryToEdit.Name = category.Name;
            categoryToEdit.Description = category.Description;
            categoryToEdit.IconPath = category.IconPath;

            await this._db.AddOrUpdateAsync(categoryToEdit);
        }

        /// <summary>
        ///     Deletes category from database
        /// </summary>
        /// <param name="guid">ID that specifies edited category</param>
        /// <returns></returns>
        public async Task DeleteCategory(Guid guid)
        {
            var categoryToDelete = await this.GetCategoryByGuid(guid);

            if (categoryToDelete == null)
            {
                return;
            }

            var defaultCategory = await this.GetDefaultCategory();
            categoryToDelete.Transactions.ToList()
                .ForEach(t => t.Category = defaultCategory);

            await this._db.DeteleAsync(categoryToDelete);
        }

        /// <summary>
        ///     Returns supported glyphicon icons for categories
        /// </summary>
        /// <returns>Glyphicons</returns>
        public static IList<string> GetGlyphicons()
        {
            // TODO: do this another way?
            // TODO: add more glyphicons
            IList<string> glyphicons = new List<string>();
            glyphicons.Add("glyphicon-record");
            glyphicons.Add("glyphicon-glass");
            glyphicons.Add("glyphicon-film");
            glyphicons.Add("glyphicon-road");
            glyphicons.Add("glyphicon-heart");
            glyphicons.Add("glyphicon-stats");
            glyphicons.Add("glyphicon-leaf");
            glyphicons.Add("glyphicon-plane");
            glyphicons.Add("glyphicon-usd");
            glyphicons.Add("glyphicon-question-sign");

            return glyphicons;
        }

        #region private

        private async Task<Category> GetDefaultCategory()
        {
            return await this._db.Categories.FirstOrDefaultAsync();
        }

        #endregion
    }
}