using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExpenseManager.BusinessLogic;
using ExpenseManager.BusinessLogic.BudgetServices;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Entity.Users;
using ExpenseManager.Resources;
using ExpenseManager.Resources.BudgetResources;
using ExpenseManager.Web.Constants;
using ExpenseManager.Web.Constants.BudgetConstants;
using ExpenseManager.Web.Helpers;
using ExpenseManager.Web.Models.Budget;
using PagedList;

namespace ExpenseManager.Web.Controllers
{
    [Authorize]
    public class BudgetController : AbstractController
    {
        private readonly BudgetService _budgetService =
            new BudgetService(ProvidersFactory.GetNewBudgetsProviders(), ProvidersFactory.GetNewTransactionsProviders());

        /// <summary>
        ///     Shows all budgets for the current UserProfile.
        /// </summary>
        /// <param name="page">Number of page which user wants to see</param>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Index(int? page)
        {
            // get Id of current logged UserProfile from HttpContext
            var userId = await this.CurrentProfileId();

            // get list of all UserProfile's budgets
            var budgets = this._budgetService.GetBudgetsByUserId(userId);
            var budgetShowModels = await budgets.ProjectTo<BudgetShowModel>().ToListAsync();

            var pageNumber = (page ?? SharedConstant.DefaultStartPage);

            return this.View(budgetShowModels.ToPagedList(pageNumber, SharedConstant.PageSize));
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
                this.AddError(SharedResource.ModelStateIsNotValid);
                return this.View(model);
            }

            // get Id of current logged UserProfile from HttpContext
            var userId = await this.CurrentProfileId();

            // finding creator by his ID
            var creator = await this._budgetService.GetBudgetCreator(userId);

            // creating new Budget by filling it from model
            var budget = this.NewBudgetInstanceFromNewBudgetModel(model, creator);

            // write budget to DB
            try
            {
                await this._budgetService.CreateBudget(budget);
            }
            catch (ServiceValidationException exception)
            {
                ModelState.AddModelErrors(exception);
                return this.View(model);
            }

            this.AddSuccess(string.Format(BudgetResource.SuccessfullCreation, budget.Name));
            return this.RedirectToAction(SharedConstant.Index);
        }

        /// <summary>
        ///     Action for editing budget
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Edit(Guid id)
        {
            // find budget by its Id
            var budget = await this._budgetService.GetBudgetById(id);

            if (budget == null)
            {
                return new HttpNotFoundResult();
            }

            return this.View(Mapper.Map<EditBudgetModel>(budget));
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
            // check if model is valid, if not, return View with it
            if (!ModelState.IsValid)
            {
                this.AddError(SharedResource.ModelStateIsNotValid);
                return this.View(model);
            }

            try
            {
                await
                    this._budgetService.EditBudget(model.Guid, model.Name, model.Description, model.Limit,
                        model.StartDate,
                        model.EndDate);
            }
            catch (ServiceValidationException exception)
            {
                ModelState.AddModelErrors(exception);
                return this.View(model);
            }

            this.AddSuccess(string.Format(BudgetResource.SuccessfullEdit, model.Name));
            return this.RedirectToAction(SharedConstant.Index);
        }

        /// <summary>
        ///     Method for displaying view with confirmation of deleting budget.
        /// </summary>
        /// <param name="id">id of budget to delete</param>
        /// <returns></returns>
        public async Task<ActionResult> Delete(Guid id)
        {
            var budget = await this._budgetService.GetBudgetById(id);

            return this.View(Mapper.Map<BudgetShowModel>(budget));
        }

        /// <summary>
        ///     Deleting budgets.
        /// </summary>
        /// <param name="model">BudgetShowModel of budget to delete</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost, ActionName(SharedConstant.Delete)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(
            [Bind(Exclude = BudgetConstant.DeleteConfirmedExcluding)] BudgetShowModel model)
        {
            if (!ModelState.IsValid)
            {
                this.AddError(SharedResource.ModelStateIsNotValid);
                return this.RedirectToAction(SharedConstant.Index);
            }

            await this._budgetService.DeleteBudget(model.Guid);

            this.AddSuccess(string.Format(BudgetResource.SuccessfullDelete, model.Name));
            return this.RedirectToAction(SharedConstant.Index);
        }

        #region private

        /// <summary>
        ///     Creates instance of Budget from NewBudgetModel and UserProfile of creator
        /// </summary>
        /// <param name="model">NewBudgetModel instance</param>
        /// <param name="creator">Creator of the budget</param>
        /// <returns></returns>
        private Budget NewBudgetInstanceFromNewBudgetModel(NewBudgetModel model, UserProfile creator)
        {
            return new Budget
            {
                Name = model.Name,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Limit = model.Limit,
                Description = model.Description ?? string.Empty,
                AccessRights =
                    new List<BudgetAccessRight>
                    {
                        new BudgetAccessRight
                        {
                            Permission = PermissionEnum.Owner,
                            UserProfile = creator
                        }
                    }
            };
        }

        #endregion
    }
}