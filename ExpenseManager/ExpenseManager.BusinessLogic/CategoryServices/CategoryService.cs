using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.BusinessLogic.Validators;
using ExpenseManager.BusinessLogic.Validators.Extensions;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Users;

namespace ExpenseManager.BusinessLogic.CategoryServices
{
    /// <summary>
    ///     Class that handles logic of CategoryController
    /// </summary>
    public class CategoryService
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
        public async Task CreateCategory(Category category, Guid userId)
        {
            category.User = await this.GetUserProfileById(userId);
            this.Validate(category);
            await this._db.AddOrUpdateAsync(category);
        }

        /// <summary>
        ///     Returns id of a user that creates the category
        /// </summary>
        /// <param name="categoryId">Id of a category</param>
        /// <returns>User id of the category creator</returns>
        public async Task<Guid> GetUserIdByCategoryId(Guid categoryId)
        {
            var category = await this.GetCategoryByGuid(categoryId);
            return category.User.Guid;
        }

        /// <summary>
        ///     Returns specified category by guid
        /// </summary>
        /// <param name="guid">ID that specifies returned category</param>
        /// <returns>Desired category</returns>
        public async Task<Category> GetCategoryByGuid(Guid guid)
        {
            return await this._db.Categories.Where(category => category.Guid.Equals(guid)).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Edits category and saves it to database
        /// </summary>
        /// <param name="category">Edited category</param>
        ///  /// <param name="userId">Edited category</param>
        /// <returns></returns>
        public async Task EditCategory(Category category, Guid userId)
        {
            category.User = await this.GetUserProfileById(userId);
            this.Validate(category);
            var categoryToEdit = await this.GetCategoryByGuid(category.Guid);
            categoryToEdit.Name = category.Name;
            categoryToEdit.Description = category.Description;
            categoryToEdit.IconPath = category.IconPath;
            categoryToEdit.User = category.User;
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
                .ForEach(transaction => transaction.Category = defaultCategory);

            await this._db.DeteleAsync(categoryToDelete);
        }

        #region private

        private async Task<Category> GetDefaultCategory()
        {
            return await this._db.Categories.FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Returns user profile by it's ID
        /// </summary>
        /// <param name="id">User profile ID</param>
        /// <returns>Desired user profile</returns>
        public async Task<UserProfile> GetUserProfileById(Guid id)
        {
            return await this._db.UserProfiles.Where(profile => profile.Guid.Equals(id)).FirstOrDefaultAsync();
        }

        #endregion
    }
}