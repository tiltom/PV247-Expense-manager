using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Web.DatabaseContexts;
using ExpenseManager.Web.Models.BudgetAccessRight;

namespace ExpenseManager.Web.Controllers
{
    public class BudgetAccessRightController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        ///     Display all budget access rights for chosen budget
        /// </summary>
        /// <param name="id">Id of chosen budget</param>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Index(Guid id)
        {
            var listOfBudgetRights =
                await this._db.BudgetAccessRights.Where(right => right.Budget.Guid == id).ToListAsync();

            return this.View(this.ConvertEntityToShowModel(listOfBudgetRights, id));
        }

        /// <summary>
        ///     Create new budget access right
        /// </summary>
        /// <param name="id">Id of budget where budget access right belongs</param>
        /// <returns></returns>
        public async Task<ActionResult> Create(Guid id)
        {
            // creating new CreateBudgetAccessRightModel instance
            var model = new CreateBudgetAccessRightModel
            {
                BudgetId = id,
                Users = await this.GetUsers()
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
            if (!ModelState.IsValid) // checking if model is valid
                return this.View(model);

            var budget = await this._db.Budgets.FindAsync(model.BudgetId); // find budget by its Id
            var assignedUser = this._db.Users.Where(user => user.Id == model.AssignedUserId).FirstOrDefault().Profile;
            // finding creator by his ID

            // creating new budget access right
            this._db.BudgetAccessRights.Add(new BudgetAccessRight
            {
                Budget = budget,
                Permission = model.Permission,
                UserProfile = assignedUser
            });

            await this._db.SaveChangesAsync();

            return this.RedirectToAction("Index", new {id = model.BudgetId});
        }

        /// <summary>
        ///     Edit chosen budget access right
        /// </summary>
        /// <param name="id">Id of budget where the budget access right belongs</param>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Edit(Guid id)
        {
            var budgetAccessRight = await this._db.BudgetAccessRights.FindAsync(id); // find BudgetAccessRight by its Id

            var model = this.ConvertEntityToEditModel(budgetAccessRight);
            // creating EditBudgetAccessRight model instance from BudgetAccessRight DB entity

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

            var budgetAccessRight = await this._db.BudgetAccessRights.FindAsync(model.Id);
            // find BudgetAccessRight by its Id

            // editing editable properties
            budgetAccessRight.Permission = model.Permission;
            budgetAccessRight.Budget = budgetAccessRight.Budget;
            budgetAccessRight.UserProfile = budgetAccessRight.UserProfile;
            await this._db.SaveChangesAsync();

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
            var budgetAccessRight = await this._db.BudgetAccessRights.FindAsync(id); // find BudgetAccessRight by its Id
            this._db.BudgetAccessRights.Remove(budgetAccessRight); // remove it from DB
            await this._db.SaveChangesAsync();

            return this.RedirectToAction("Index", new {id = budgetId});
        }

        #region private

        /// <summary>
        ///     Get all access rights for current budget and convert it to the list of ShowBudgetAccessRightModel instances
        /// </summary>
        /// <param name="entity">List of BudgeAccessRight entities</param>
        /// <param name="BudgetId">Id of budget</param>
        /// <returns>ShowBudgetAccessRightModel instances</returns>
        private List<ShowBudgetAccessRightModel> ConvertEntityToShowModel(List<BudgetAccessRight> entity, Guid BudgetId)
        {
            var list = new List<ShowBudgetAccessRightModel>();

            // iterating over all BudgetAccessRight entities and mapping them to the ShowBudgetAccessRightModel
            foreach (var item in entity)
            {
                list.Add(new ShowBudgetAccessRightModel
                {
                    AssignedUserName = item.UserProfile.FirstName,
                    BudgetId = BudgetId,
                    Id = item.Guid,
                    Permission = item.Permission
                });
            }

            return list;
        }

        /// <summary>
        ///     Converts BudgetAccessRight DB entity to EditBudgetAccessRightModel
        /// </summary>
        /// <param name="entity">Instance of BudgetAccessRight</param>
        /// <returns>Instance of EditBudgetAccessRightModel</returns>
        private EditBudgetAccessRightModel ConvertEntityToEditModel(BudgetAccessRight entity)
        {
            // mapping properties from BudgetAccessRight DB entity to EditBudgetAccessRightModel
            return new EditBudgetAccessRightModel
            {
                AssignedUserId = entity.UserProfile.Guid,
                AssignedUserName = entity.UserProfile.FirstName,
                Permission = entity.Permission,
                BudgetId = entity.Budget.Guid,
                Id = entity.Guid
            };
        }

        /// <summary>
        ///     Get all users from DB
        /// </summary>
        /// <returns>List of all users</returns>
        private async Task<List<SelectListItem>> GetUsers()
        {
            return
                await this._db.Users.Select(user => new SelectListItem {Value = user.Id, Text = user.UserName})
                    .ToListAsync();
        }

        #endregion
    }
}