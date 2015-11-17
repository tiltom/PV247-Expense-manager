using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExpenseManager.BusinessLogic;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Web.Models.Category;

namespace ExpenseManager.Web.Controllers
{
    public class CategoryController : AbstractController
    {
        private readonly CategoryService _categoryService = new CategoryService();

        /// <summary>
        ///     Shows all existing categories.
        /// </summary>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Index()
        {
            var categories = this._categoryService.GetCategories();
            var categoryShowModels = await categories.ProjectTo<CategoryShowModel>().OrderBy(x => x.Name).ToListAsync();

            return this.View(categoryShowModels);
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
                var newCategory = Mapper.Map<Category>(category);
                await this._categoryService.CreateCategory(newCategory);

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
        public ActionResult Edit(Guid guid)
        {
            // find category by its Id
            var category = this._categoryService.GetCategoryByGuid(guid);

            if (category == null)
            {
                return new HttpNotFoundResult();
            }

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
        ///     Action for deleting categories.
        /// </summary>
        /// <param name="guid">Id of category to delete</param>
        /// <returns>Redirect to Index</returns>
        public async Task<ActionResult> Delete(Guid guid)
        {
            await this._categoryService.DeleteCategory(guid);

            return this.RedirectToAction("Index");
        }

        #region protected

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //this._db.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}