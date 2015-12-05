using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExpenseManager.BusinessLogic.CategoryServices;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Web.Models.Category;

namespace ExpenseManager.Web.Controllers
{
    public class CategoryController : AbstractController
    {
        private readonly CategoryService _categoryService =
            new CategoryService(ProvidersFactory.GetNewTransactionsProviders());

        /// <summary>
        ///     Shows all existing categories.
        /// </summary>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Index()
        {
            var categories = this._categoryService.GetCategories();
            var categoryShowModels = await
                categories.ProjectTo<CategoryShowModel>(categories).OrderBy(x => x.Name).ToListAsync();

            return this.View(categoryShowModels);
        }

        /// <summary>
        ///     Creates new category
        /// </summary>
        /// <returns>View with model</returns>
        [HttpGet]
        public ActionResult Create()
        {
            var glyphicons = CategoryService.GetGlyphicons();
            ViewBag.Glyphicons = glyphicons;
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
                var newCategory = Mapper.Map<Category>(category);
                await this._categoryService.CreateCategory(newCategory);

                return this.RedirectToAction("Index");
            }

            // TODO: add error message to layout and display it here
            return this.View(category);
        }

        /// <summary>
        ///     Action for editing category
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>View with model</returns>
        [HttpGet]
        public async Task<ActionResult> Edit(Guid guid)
        {
            // find category by its Id
            var category = await this._categoryService.GetCategoryByGuid(guid);

            var glyphicons = CategoryService.GetGlyphicons();
            ViewBag.Glyphicons = glyphicons;

            return this.View(Mapper.Map<CategoryShowModel>(category));
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
                var editedCategory = Mapper.Map<Category>(category);
                await this._categoryService.EditCategory(editedCategory);

                return this.RedirectToAction("Index");
            }

            return this.View();
        }

        /// <summary>
        ///     Method for displaying view with confirmation of deleting category.
        /// </summary>
        /// <param name="id">id of category to be deleted</param>
        /// <returns></returns>
        public async Task<ActionResult> Delete(Guid id)
        {
            // find category by its Id
            var category = await this._categoryService.GetCategoryByGuid(id);

            return this.View(Mapper.Map<CategoryShowModel>(category));
        }

        /// <summary>
        ///     Action for deleting categories.
        /// </summary>
        /// <param name="model">CategoryShowModel of category to delete</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(CategoryShowModel model)
        {
            if (!ModelState.IsValid)
            {
                // error
                return this.RedirectToAction("Index");
            }

            await this._categoryService.DeleteCategory(model.Guid);

            return this.RedirectToAction("Index");
        }
    }
}