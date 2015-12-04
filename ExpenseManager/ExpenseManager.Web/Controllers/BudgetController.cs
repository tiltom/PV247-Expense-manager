using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExpenseManager.BusinessLogic.BudgetServices;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Providers.Factory;
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

            var pageSize = 5;
            var pageNumber = (page ?? 1);

            return this.View(budgetShowModels.ToPagedList(pageNumber, pageSize));
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
            if (!BudgetService.ValidateModel(model.StartDate, model.EndDate))
            {
                // TODO: add error message to layout and display it here
                return this.View(model);
            }

            // get Id of current logged UserProfile from HttpContext
            var userId = await this.CurrentProfileId();

            // finding creator by his ID
            var creator = await this._budgetService.GetBudgetCreator(userId);

            // creating new Budget by filling it from model
            var budget = new Budget
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
            };

            // write budget to DB
            await this._budgetService.CreateBudget(budget);

            return this.RedirectToAction("Index");
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
            // check if model is valid, if not, return View with it (and error message later)
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            // check if model is valid
            if (!BudgetService.ValidateModel(model.StartDate, model.EndDate))
            {
                // TODO: add error message to layout and display it here
                return this.View(model);
            }

            await
                this._budgetService.EditBudget(model.Guid, model.Name, model.Description, model.Limit, model.StartDate,
                    model.EndDate);

            // Add OK message
            return this.RedirectToAction("Index");
        }

        /// <summary>
        ///     Deleting budgets.
        /// </summary>
        /// <param name="id">Id of budget to delete</param>
        /// <returns>Redirect to Index</returns>
        public async Task<ActionResult> Delete(Guid id)
        {
            await this._budgetService.DeleteBudget(id);

            return this.RedirectToAction("Index");
        }
    }
}