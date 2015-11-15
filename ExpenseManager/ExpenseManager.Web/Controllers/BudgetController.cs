﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExpenseManager.BusinessLogic;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Web.Models.Budget;

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
            var budgetShowModels =
                await
                    this._db.Budgets.Where(user => user.Creator.Guid == userId)
                        .ProjectTo<BudgetShowModel>()
                        .ToListAsync();

            return this.View(budgetShowModels);
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
        public async Task<ActionResult> Edit(Guid id)
        {
            // find budget by its Id

            var budget = await this._db.Budgets.Where(b => b.Guid == id).FirstOrDefaultAsync();


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
        public async Task<ActionResult> Delete(Guid id)
        {
            // find budget to delete by its Id
            var budget = await this._db.Budgets.Where(b => b.Guid.Equals(id)).FirstOrDefaultAsync();
            // delete connections to this budget in BudgetAccessRight table -
            // TODO inspect if cascade won't apply
            // budget.AccessRights.ToList().ForEach(r => this._db.BudgetAccessRights.Remove(r));
            // removing budget
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
    }
}