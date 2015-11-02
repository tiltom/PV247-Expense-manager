using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Web.DatabaseContexts;
using ExpenseManager.Web.Models.Category;

namespace ExpenseManager.Web.Controllers
{
    public class CategoryController : AbstractController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        ///     Shows all existing categories.
        /// </summary>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Index()
        {
            var categories = await this._db.Categories.ToListAsync();

            return this.View(this.ConvertEntityListToCategoryShowModelList(categories));
        }

        /// <summary>
        ///     Creates new category
        /// </summary>
        /// <returns>View with model</returns>
        [HttpGet]
        public ActionResult Create()
        {
            return this.View();
        }

        /// <summary>
        ///     Creates new category
        /// </summary>
        /// <param name="category">CategoryShowModel instance</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CategoryShowModel category)
        {
            // first, check if model is valid
            if (ModelState.IsValid)
            {
                var newCategory = this.CreateCategoryFromCategoryShowModel(category);

                this._db.Categories.Add(newCategory);

                await this._db.SaveChangesAsync();

                return this.RedirectToAction("Index");
            }

            // TODO: add error message to layout and display it here
            return this.View();
        }

        /// <summary>
        ///     Action for editing category
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>View with model</returns>
        [HttpGet]
        public async Task<ActionResult> Edit(Guid? guid)
        {
            // check if guid is not null - it can happen by calling this action without /Guid
            if (guid == null)
            {
                return this.RedirectToAction("Index"); // TODO add error message
            }

            // find category by its Id
            var category = await this._db.Categories.FindAsync(guid);

            return this.View(this.CreateCategoryShowModelFromCategory(category));
        }

        /// <summary>
        ///     Editing of category
        /// </summary>
        /// <param name="category">Instance of CategoryShowModel</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CategoryShowModel category)
        {
            // check if model is valid
            if (ModelState.IsValid)
            {
                // find category by its Id from model
                var categoryToEdit = await this._db.Categories.FindAsync(category.Guid);

                // editing editable properties, TODO: refactor it
                categoryToEdit.Description = category.Description;
                categoryToEdit.IconPath = category.Icon;
                categoryToEdit.Name = categoryToEdit.Name;

                await this._db.SaveChangesAsync();

                return this.RedirectToAction("Index");
            }

            return this.View();
        }

        /// <summary>
        ///     Action for deleting categories.
        /// </summary>
        /// <param name="guid">Id of category to delete</param>
        /// <returns>Redirect to Index</returns>
        public async Task<ActionResult> Delete(Guid? guid)
        {
            // check if guid is not null - it can happen by calling this action without /Guid
            if (guid == null)
            {
                return this.RedirectToAction("Index"); // TODO add error message
            }

            // find category to delete by its Id
            var categoryToDelete = await this._db.Categories.FindAsync(guid);

            // get the default currency
            var defaultCategory = await this.GetDefaultCategory();
            // delete connections to this category in Transactions table and set the category to Default
            categoryToDelete.Transactions.ToList()
                .ForEach(t => t.Category = defaultCategory);

            // delete the category
            this._db.Categories.Remove(categoryToDelete);
            await this._db.SaveChangesAsync();

            return this.RedirectToAction("Index");
        }

        #region protected

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._db.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region private

        /// <summary>
        ///     Converts list of entities of type Category into CategoryShowModel.
        /// </summary>
        /// <param name="categories">List of entities of type Category</param>
        /// <returns>
        ///     List of CategoryShowModel
        /// </returns>
        private List<CategoryShowModel> ConvertEntityListToCategoryShowModelList(List<Category> categories)
        {
            var categoryShowModelList = new List<CategoryShowModel>();

            // iterating over all Category entities and mapping them to the CategoryShowModel
            foreach (var item in categories)
            {
                categoryShowModelList.Add(new CategoryShowModel
                {
                    Guid = item.Guid,
                    Name = item.Name,
                    Icon = item.IconPath,
                    Description = item.Description
                });
            }

            return categoryShowModelList;
        }

        /// <summary>
        ///     Converts CategoryShowModel entity into Category entity.
        /// </summary>
        /// <param name="showModel">CategoryShowModel entity</param>
        /// <returns>
        ///     Category entity
        /// </returns>
        private Category CreateCategoryFromCategoryShowModel(CategoryShowModel showModel)
        {
            return new Category
            {
                Name = showModel.Name,
                IconPath = showModel.Icon,
                Description = showModel.Description
            };
        }

        /// <summary>
        ///     Converts Category entity into CategoryShowModel entity.
        /// </summary>
        /// <param name="category">Category entity</param>
        /// <returns>
        ///     CategoryShowModel entity
        /// </returns>
        private CategoryShowModel CreateCategoryShowModelFromCategory(Category category)
        {
            return new CategoryShowModel
            {
                Name = category.Name,
                Icon = category.IconPath,
                Description = category.Description
            };
        }

        #endregion
    }
}