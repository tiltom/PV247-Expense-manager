﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Web.Models.BudgetAccessRight;

namespace ExpenseManager.Web.Controllers
{
    public class BudgetAccessRightController : AbstractController
    {
        private readonly IBudgetsProvider _db = ProvidersFactory.GetNewBudgetsProviders();

        /// <summary>
        ///     Display all budget access rights for chosen budget
        /// </summary>
        /// <param name="id">Id of chosen budget</param>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Index(Guid id)
        {
            var accessRightModels =
                await
                    this._db.BudgetAccessRights.Where(right => right.Budget.Guid == id)
                        .ProjectTo<ShowBudgetAccessRightModel>()
                        .ToListAsync();

            accessRightModels.ForEach(model => model.BudgetId = id);

            return this.View(accessRightModels);
        }

        /// <summary>
        ///     Create new budget access right
        /// </summary>
        /// <param name="id">Id of budget where budget access right belongs</param>
        /// <returns></returns>
        public async Task<ActionResult> Create(Guid id)
        {
            // find access rights to the budget
            var budgetAccessRight =
                this._db.Budgets.Where(b => b.Guid.Equals(id)).FirstOrDefaultAsync().Result.AccessRights;

            // save Guids of users with access rights to the List
            var usersList = budgetAccessRight.Select(item => item.UserProfile.Guid).ToList();

            // creating new CreateBudgetAccessRightModel instance
            var model = new CreateBudgetAccessRightModel
            {
                BudgetId = id,
                Permissions = this.GetPermissions(),
                Users = await this.GetUsers(usersList)
            };

            return this.View(model);
        }

        /// <summary>
        ///     Create new budget access right
        /// </summary>
        /// <param name="model">CreateBudgetAccessRightModel instance</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateBudgetAccessRightModel model)
        {
            // checking if model is valid
            if (!ModelState.IsValid)
                return this.View(model);

            // find budget by its Id
            var budget = await this._db.Budgets.Where(b => b.Guid.Equals(model.BudgetId)).FirstOrDefaultAsync();

            // finding creator by his ID
            var assignedUser =
                this._db.UserProfiles.FirstOrDefault(user => user.Guid.ToString() == model.AssignedUserId);

            // creating new budget access right
            await this._db.AddOrUpdateAsync(new BudgetAccessRight
            {
                Budget = budget,
                Permission = model.Permission,
                UserProfile = assignedUser
            });

            return this.RedirectToAction("Index", new {id = model.BudgetId});
        }

        /// <summary>
        ///     Edit chosen budget access right
        /// </summary>
        /// <param name="id">Id of budget where the budget access right belongs</param>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Edit(Guid id)
        {
            // find BudgetAccessRight by its Id
            var budgetAccessRight =
                await this._db.BudgetAccessRights.Where(b => b.Guid.Equals(id)).FirstOrDefaultAsync();

            // creating EditBudgetAccessRight model instance from BudgetAccessRight DB entity
            var model = Mapper.Map<EditBudgetAccessRightModel>(budgetAccessRight);
            model.Permissions = this.GetPermissions();

            return this.View(model);
        }

        /// <summary>
        ///     Edit chosen budget access right
        /// </summary>
        /// <param name="model">EditBudgetAccessRightModel instance</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditBudgetAccessRightModel model)
        {
            if (!ModelState.IsValid) // checking if model is valid
                return this.View(model);

            // find BudgetAccessRight by its Id
            var budgetAccessRight =
                await this._db.BudgetAccessRights.Where(b => b.Guid.Equals(model.Id)).FirstOrDefaultAsync();

            // editing editable properties
            budgetAccessRight.Permission = model.Permission;
            budgetAccessRight.Budget = budgetAccessRight.Budget;
            budgetAccessRight.UserProfile = budgetAccessRight.UserProfile;

            return this.RedirectToAction("Index", new {id = model.BudgetId});
        }

        /// <summary>
        ///     Delete budget access right from DB
        /// </summary>
        /// <param name="id">Id of budget access right to delete</param>
        /// <param name="budgetId">Id of budget where the budget access right belongs</param>
        /// <returns>Redirect to Index</returns>
        public async Task<ActionResult> Delete(Guid id, Guid budgetId)
        {
            // find BudgetAccessRight by its Id
            var budgetAccessRight =
                await this._db.BudgetAccessRights.Where(b => b.Guid.Equals(id)).FirstOrDefaultAsync();
            if (budgetAccessRight.Permission.Equals(PermissionEnum.Owner))
            {
                return this.RedirectToAction("Index", new {id = budgetId});
            }

            // remove it from DB
            await this._db.DeteleAsync(budgetAccessRight);

            return this.RedirectToAction("Index", new {id = budgetId});
        }

        #region private

        /// <summary>
        ///     Get list of users which don't have permissions to the budget yet.
        /// </summary>
        /// <param name="users">List of Guids of users with permission</param>
        /// <returns></returns>
        private async Task<List<SelectListItem>> GetUsers(ICollection<Guid> users)
        {
            // Id of current user
            var currrentUserId = await this.CurrentProfileId();
            return
                await
                    this._db.UserProfiles
                        .Where(
                            u => // filtering users which don't have owner permission or other permissions
                                u.BudgetAccessRights.All(war => war.Budget.Creator.Guid != currrentUserId) ||
                                !users.Contains(u.Guid))
                        .Select(user => new SelectListItem {Value = user.Guid.ToString(), Text = user.FirstName})
                        .ToListAsync();
        }

        #endregion
    }
}