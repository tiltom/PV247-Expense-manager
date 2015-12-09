using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExpenseManager.BusinessLogic;
using ExpenseManager.BusinessLogic.CategoryServices;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Resources;
using ExpenseManager.Web.Helpers;
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
                categories.ProjectTo<CategoryModel>(categories).OrderBy(category => category.Name).ToListAsync();

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
            return this.View(new CategoryModel
            {
                Glyphicons = glyphicons,
                Icon = glyphicons.ElementAt(0)
            });
        }

        /// <summary>
        ///     Creates new category
        /// </summary>
        /// <param name="category">CategoryShowModel instance</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CategoryModel category)
        {
            // first, check if model is valid
            if (!ModelState.IsValid)
            {
                this.AddError(SharedResource.ModelStateIsNotValid);
                category.Glyphicons = CategoryService.GetGlyphicons();
                return this.View(category);
            }

            var newCategory = Mapper.Map<Category>(category);

            try
            {
                await this._categoryService.CreateCategory(newCategory);
            }
            catch (ServiceValidationException exception)
            {
                ModelState.AddModelErrors(exception);
                category.Glyphicons = CategoryService.GetGlyphicons();
                return this.View(category);
            }

            return this.RedirectToAction("Index");
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

            var viewModel = Mapper.Map<CategoryModel>(category);
            viewModel.Glyphicons = CategoryService.GetGlyphicons();

            return this.View(viewModel);
        }

        /// <summary>
        ///     Editing of category
        /// </summary>
        /// <param name="category">Instance of CategoryShowModel</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CategoryModel category)
        {
            // check if model is valid
            if (!ModelState.IsValid)
            {
                this.AddError(SharedResource.ModelStateIsNotValid);
                category.Glyphicons = CategoryService.GetGlyphicons();
                return this.View(category);
            }

            var editedCategory = Mapper.Map<Category>(category);

            try
            {
                await this._categoryService.EditCategory(editedCategory);
            }
            catch (ServiceValidationException exception)
            {
                ModelState.AddModelErrors(exception);
                category.Glyphicons = CategoryService.GetGlyphicons();
                return this.View(category);
            }

            return this.RedirectToAction("Index");
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

            return this.View(Mapper.Map<CategoryModel>(category));
        }

        /// <summary>
        ///     Action for deleting categories.
        /// </summary>
        /// <param name="model">CategoryShowModel of category to delete</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(CategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                this.AddError(SharedResource.ModelStateIsNotValid);
                return this.RedirectToAction("Index");
            }

            await this._categoryService.DeleteCategory(model.Guid);

            return this.RedirectToAction("Index");
        }
    }
}