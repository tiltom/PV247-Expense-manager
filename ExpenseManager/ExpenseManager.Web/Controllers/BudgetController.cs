using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Web.DatabaseContexts;
using ExpenseManager.Web.Models.Budget;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Entity.Providers;

namespace ExpenseManager.Web.Controllers
{
    [Authorize]
    public class BudgetController : AbstractController
    {

        private readonly IBudgetsProvider _db = ProvidersFactory.GetNewBudgetsProviders();

        /// <summary>
        ///     Shows all budgets for the current UserProfile.
        /// </summary>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Index()
        {
            // get Id of current logged UserProfile from HttpContext
            var userId = await this.CurrentProfileId();

            // get list of all UserProfile's budgets
            var budgetList = await this._db.Budgets.Where(user => user.Creator.Guid == userId).ToListAsync();

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
            // check if model is valid
            if (!ModelState.IsValid)
            {
                // TODO: add error message to layout and display it here
                return this.View(model);
            }

            // check if model is valid
            if (!this.ValidateModel(model.StartDate, model.EndDate))
            {
                // TODO: add error message to layout and display it here
                return this.View(model);
            }

            // get Id of current logged UserProfile from HttpContext
            var userId = await this.CurrentProfileId();

            // finding creator by his ID
            var creator = await this._db.UserProfiles.FirstOrDefaultAsync(user => user.Guid == userId);

            // creating new Budget by filling it from model
            await this._db.AddOrUpdateAsync(new Budget
            {
                Name = model.Name,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Limit = model.Limit,
                Description = model.Description ?? string.Empty,
                Creator = creator,
                Currency = await this.GetDefaultCurrency(),
                AccessRights =
                    new List<BudgetAccessRight>
                    {
                        new BudgetAccessRight
                        {
                            Permission = PermissionEnum.Owner,
                            UserProfile = creator
                        }
                    }
            });

            return this.RedirectToAction("Index");
        }

        /// <summary>
        ///     Action for editing budget
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Edit(Guid? id)
        {
            // check if Id is not null - it can happen by calling this action without /Guid
            if (id == null)
            {
                return this.RedirectToAction("Index"); // TODO add error message
            }

            // find budget by its Id
            var budget = await this._db.Budgets.Where(b => b.Guid.Equals((Guid)id)).FirstOrDefaultAsync();
            

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
            // check if model is valid, if not, return View with it (and error message later)
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            // check if model is valid
            if (!this.ValidateModel(model.StartDate, model.EndDate))
            {
                // TODO: add error message to layout and display it here
                return this.View(model);
            }

            // find budget by its Id from model
            var budget = await this._db.Budgets.Where(b => b.Guid.Equals(model.Guid)).FirstOrDefaultAsync();

            // editing editable properties, TODO: refactor it
            budget.Name = model.Name;
            budget.StartDate = model.StartDate;
            budget.EndDate = model.EndDate;
            budget.Description = model.Description;
            budget.Limit = model.Limit;
            budget.Creator = budget.Creator;
            budget.AccessRights = budget.AccessRights;
            budget.Currency = budget.Currency;

            await this._db.AddOrUpdateAsync(budget);

            // Add OK message
            return this.RedirectToAction("Index");
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
                // TODO add error message
                return this.RedirectToAction("Index");
            }
            
            var budget = await this._db.Budgets.Where(b => b.Guid.Equals((Guid)id)).FirstOrDefaultAsync();
            await this._db.DeteleAsync(budget);

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

        #region private

        /// <summary>
        ///     Converts list of entities of type Budget into BudgetShowModel.
        /// </summary>
        /// <param name="entities">List of entities of type Budget</param>
        /// <returns>
        ///     List of BudgetShowModel
        /// </returns>
        private IEnumerable<BudgetShowModel> ConvertEntityToBudgetShowModel(List<Budget> entities)
        {
            foreach (var item in entities)
            {
                yield return
                    new BudgetShowModel
                    {
                        Guid = item.Guid,
                        Name = item.Name,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        Limit = item.Limit
                    };
            }
        }

        /// <summary>
        ///     Additional validation for model
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private bool ValidateModel(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                return false;

            return true;
        }

        #endregion
    }
}