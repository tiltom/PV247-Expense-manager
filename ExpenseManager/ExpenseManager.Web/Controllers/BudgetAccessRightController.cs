using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExpenseManager.BusinessLogic;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Web.Models.BudgetAccessRight;
using PagedList;

namespace ExpenseManager.Web.Controllers
{
    public class BudgetAccessRightController : AbstractController
    {
        private readonly BudgetAccessRightService _budgetAccessRightService = new BudgetAccessRightService();
        private readonly IBudgetsProvider _db = ProvidersFactory.GetNewBudgetsProviders();

        /// <summary>
        ///     Display all budget access rights for chosen budget
        /// </summary>
        /// <param name="id">Id of chosen budget</param>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Index(Guid id, int? page)
        {
            var accessRightModels =
                await
                    this._budgetAccessRightService.GetAccessRightsByBudgetId(id)
                        .ProjectTo<ShowBudgetAccessRightModel>()
                        .ToListAsync();

            accessRightModels.ForEach(model => model.BudgetId = id);

            var pageSize = 5;
            var pageNumber = (page ?? 1);

            return this.View(accessRightModels.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        ///     Create new budget access right
        /// </summary>
        /// <param name="id">Id of budget where budget access right belongs</param>
        /// <returns></returns>
        public async Task<ActionResult> Create(Guid id)
        {
            var usersList = this._budgetAccessRightService.GetUsersGuidListForBudget(id);

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

            await
                this._budgetAccessRightService.CreateAccessBudgetRight(model.BudgetId, model.AssignedUserId,
                    model.Permission);

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
            if (!ModelState.IsValid) // checking if model is valid
                return this.View(model);

            await this._budgetAccessRightService.EditBudgetAccessRight(Mapper.Map<BudgetAccessRight>(model));

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
            await this._budgetAccessRightService.DeleteBudgetAccessRight(id);

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