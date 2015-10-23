using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Web.Models.Budget;
using ExpenseManager.Web.Models.User;
using Microsoft.AspNet.Identity;

namespace ExpenseManager.Web.Controllers
{
    [Authorize]
    public class BudgetController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext(); // instance of DB context

        /// <summary>
        ///     Shows all budgets for the current user.
        /// </summary>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Index()
        {
            var userId = HttpContext.User.Identity.GetUserId(); // get Id of current logged user from HttpContext

            // get list of all user's budgets
            var budgetList = await this._db.Budgets.Where(user => user.Creator.Id == userId).ToListAsync();

            return this.View(this.ConvertEntityToBudgetShowModel(budgetList));
        }

        /// <summary>
        ///     Creates new budget
        /// </summary>
        /// <returns>View with model</returns>
        public ActionResult Create()
        {
            var model = new NewBudgetModel();

            return this.View(model);
        }

        /// <summary>
        ///     Creates new budget
        /// </summary>
        /// <param name="model">NewBudgetModel instance</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(NewBudgetModel model)
        {
            if (!ModelState.IsValid) // check if model is valid
            {
                return this.View(model); // TODO: add error message to layout and display it here
            }
            var userId = HttpContext.User.Identity.GetUserId(); // get Id of current logged user from HttpContext

            var creator = this._db.Users.Where(user => user.Id == userId).FirstOrDefault(); // finding creator by his ID

            // creating new Budget by filling it from model
            this._db.Budgets.Add(new Budget
            {
                //Guid = Guid.NewGuid(),
                Name = model.Name,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Limit = model.Limit,
                Description = model.Description ?? string.Empty,
                Creator = creator,
                Currency = new Currency {Name = "Česká koruna", Symbol = "Kč"},
                AccessRights =
                    new List<BudgetAccessRight>
                    {
                        new BudgetAccessRight
                        {
                            Permission = PermissionEnum.Owner,
                            User = creator
                        }
                    }
            });

            await this._db.SaveChangesAsync();

            return this.RedirectToAction("Index");
        }

        /// <summary>
        ///     Action for editing budget
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null) // check if Id is not null - it can happen by calling this action without /Guid
            {
                return this.RedirectToAction("Index"); // TODO add error message
            }

            var budget = await this._db.Budgets.FindAsync(id); // find budget by its Id

            // filling model from DB entity
            var model = new EditBudgetModel
            {
                Name = budget.Name,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                Description = budget.Description,
                Limit = budget.Limit,
                Guid = budget.Guid
            };

            return this.View(model);
        }

        /// <summary>
        ///     Editing of budget.
        /// </summary>
        /// <param name="model">Instance of EditBudgetModel</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditBudgetModel model)
        {
            if (!ModelState.IsValid) // check if model is valid, if not, return View with it (and error message later)
            {
                return this.View(model);
            }

            var budget = await this._db.Budgets.FindAsync(model.Guid); // find budget by its Id from model

            // editing editable properties
            budget.Name = model.Name;
            budget.StartDate = model.StartDate;
            budget.EndDate = model.EndDate;
            budget.Description = model.Description;
            budget.Limit = model.Limit;


            await this._db.SaveChangesAsync();

            return this.RedirectToAction("Index"); // Add OK message
        }

        /// <summary>
        ///     Deleting budgets.
        /// </summary>
        /// <param name="id">Id of budget to delete</param>
        /// <returns>Redirect to Index</returns>
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return this.RedirectToAction("Index"); // TODO add error message
            }

            var budget = await this._db.Budgets.FindAsync(id); // find budget to delete by its Id

            budget.AccessRights.ToList().ForEach(r => this._db.BudgetAccessRights.Remove(r));
            // delete connections to this budget in BudgetAccessRight table

            this._db.Budgets.Remove(budget); // removing budget
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
        ///     Converts list of entities of type Budget into BudgetShowModel.
        /// </summary>
        /// <param name="entities">List of entities of type Budget</param>
        /// <returns>
        ///     List of BudgetShowModel
        /// </returns>
        private List<BudgetShowModel> ConvertEntityToBudgetShowModel(List<Budget> entities)
        {
            var budgetShowModelList = new List<BudgetShowModel>();

            // iterating over all Budget entities and mapping them to the BudgetShowModel
            foreach (var item in entities)
            {
                budgetShowModelList.Add(new BudgetShowModel
                {
                    Guid = item.Guid,
                    Name = item.Name,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    Limit = item.Limit
                });
            }

            return budgetShowModelList;
        }

        #endregion
    }
}