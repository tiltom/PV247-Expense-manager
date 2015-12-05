﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CaptchaMvc.HtmlHelpers;
using ExpenseManager.BusinessLogic.BudgetServices;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Resources;
using ExpenseManager.Web.Models.BudgetAccessRight;
using PagedList;

namespace ExpenseManager.Web.Controllers
{
    [Authorize]
    public class BudgetAccessRightController : AbstractController
    {
        private readonly BudgetAccessRightService _budgetAccessRightService
            = new BudgetAccessRightService(ProvidersFactory.GetNewBudgetsProviders());

        /// <summary>
        ///     Display all budget access rights for chosen budget
        /// </summary>
        /// <param name="id">Id of chosen budget</param>
        /// <param name="page">Number of page which user wants to see</param>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Index(Guid id, int? page)
        {
            var accessRightModels =
                await
                    this._budgetAccessRightService.GetAccessRightsByBudgetId(id)
                        .ProjectTo<ShowBudgetAccessRightModel>()
                        .ToListAsync();

            accessRightModels.ForEach(model => model.BudgetId = id);
            var pageNumber = page ?? 1;
            return this.View(accessRightModels.ToPagedList(pageNumber, PageSize));
        }

        /// <summary>
        ///     Create new budget access right
        /// </summary>
        /// <param name="id">Id of budget where budget access right belongs</param>
        /// <returns></returns>
        public ActionResult Create(Guid id)
        {
            // creating new CreateBudgetAccessRightModel instance
            var model = new CreateBudgetAccessRightModel
            {
                BudgetId = id,
                Permissions = this.GetPermissions()
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
            this.IsCaptchaValid(SharedResource.CaptchaValidationFailed);
            // checking if model is valid
            if (ModelState.IsValid)
            {
                var userId = await this.GetUserProfileByEmail(model.AssignedUserEmail);
                if (!userId.Equals(Guid.Empty))
                {
                    await
                        this._budgetAccessRightService.CreateAccessBudgetRight(
                            model.BudgetId,
                            userId,
                            model.Permission
                            );
                    return this.RedirectToAction("Index", new {id = model.BudgetId});
                }
                ModelState.AddModelError("UserProfile", SharedResource.UserNotFoundByEmail);
            }
            model.Permissions = this.GetPermissions();
            return this.View(model);
        }

        /// <summary>
        ///     Edit chosen budget access right
        /// </summary>
        /// <param name="id">Id of budget where the budget access right belongs</param>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Edit(Guid id)
        {
            // find BudgetAccessRight by its Id
            var budgetAccessRight = await this._budgetAccessRightService.GetBudgetAccessRightById(id);

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
            // checking if model is valid
            if (!ModelState.IsValid)
                return this.View(model);

            await this._budgetAccessRightService.EditBudgetAccessRight(model.Id, model.Permission, model.AssignedUserId);

            return this.RedirectToAction("Index", new {id = model.BudgetId});
        }

        /// <summary>
        ///     Method for displaying view with confirmation of deleting budget access right.
        /// </summary>
        /// <param name="id">id of budget access right to delete</param>
        /// <returns></returns>
        public async Task<ActionResult> Delete(Guid id, Guid budgetId)
        {
            // find BudgetAccessRight by its Id
            var budgetAccessRight = await this._budgetAccessRightService.GetBudgetAccessRightById(id);

            var model = Mapper.Map<ShowBudgetAccessRightModel>(budgetAccessRight);
            model.BudgetId = budgetId;

            return this.View(model);
        }

        /// <summary>
        ///     Delete budget access right from DB
        /// </summary>
        /// <param name="model">ShowBudgetAccessRightModel of budget access right to delete</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(ShowBudgetAccessRightModel model)
        {
            if (!ModelState.IsValid)
            {
                // error
                return this.RedirectToAction("Index", new {id = model.BudgetId});
            }

            await this._budgetAccessRightService.DeleteBudgetAccessRight(model.Id);

            return this.RedirectToAction("Index", new {id = model.BudgetId});
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
            var currentUserId = await this.CurrentProfileId();

            var userProfiles = this._budgetAccessRightService.GetUserProfiles(users, currentUserId);

            return
                await
                    userProfiles
                        .Select(user => new SelectListItem {Value = user.Guid.ToString(), Text = user.FirstName})
                        .ToListAsync();
        }

        #endregion
    }
}