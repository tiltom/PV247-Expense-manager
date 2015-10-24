using System;
using System.Linq;
using System.Web.Mvc;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Web.DatabaseContexts;

namespace ExpenseManager.Web.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index()
        {
            using (var context = ApplicationDbContext.Create())
            {
                var categories = context.Categories.ToList();

                return this.View(categories);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                using (var context = ApplicationDbContext.Create())
                {
                    context.Categories.Add(category);
                    context.SaveChanges();
                }

                return this.RedirectToAction("Index");
            }

            return this.View();
        }

        [HttpGet]
        public ActionResult Edit(Guid guid)
        {
            using (var context = ApplicationDbContext.Create())
            {
                var category = context.Categories.Find(guid);

                return this.View(category);
            }
        }

        [HttpPost]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                using (var context = ApplicationDbContext.Create())
                {
                    var categoryToEdit = context.Categories.Find(category.Guid);
                    categoryToEdit.Description = category.Description;
                    categoryToEdit.IconPath = category.IconPath;
                    categoryToEdit.Name = categoryToEdit.Name;
                    context.SaveChanges();
                }

                return this.RedirectToAction("Index");
            }

            return this.View();
        }

        public ActionResult Delete(Guid guid)
        {
            using (var context = ApplicationDbContext.Create())
            {
                var categoryToEdit = context.Categories.Find(guid);
                context.Categories.Remove(categoryToEdit);
                context.SaveChanges();

                return this.RedirectToAction("Index");
            }
        }
    }
}