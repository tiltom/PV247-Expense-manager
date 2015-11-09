using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
            var categoryShowModels = await this._db.Categories.ProjectTo<CategoryShowModel>().ToListAsync();

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
            // find category by its Id
            var category = await this._db.Categories.FirstOrDefaultAsync(x => x.Guid == guid);

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
                // find category by its Id from model
                var categoryToEdit = await this._db.Categories.FindAsync(category.Guid);

                // editing editable properties, TODO: refactor this
                categoryToEdit.Description = category.Description;
                categoryToEdit.IconPath = category.Icon;
                categoryToEdit.Name = category.Name;
                categoryToEdit.Type = category.Type;

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
            // find category to delete by its Id
            var categoryToDelete = await this._db.Categories.FindAsync(guid);

            if (categoryToDelete == null)
            {
                return new HttpNotFoundResult();
            }

            // get the default category
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
    }
}