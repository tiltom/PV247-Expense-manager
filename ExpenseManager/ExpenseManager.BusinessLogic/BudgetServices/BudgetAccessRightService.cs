using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.BusinessLogic.Validators;
using ExpenseManager.BusinessLogic.Validators.Extensions;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Users;

namespace ExpenseManager.BusinessLogic.BudgetServices
{
    /// <summary>
    ///     Class that handles logic of BudgetAccessRightController
    /// </summary>
    public class BudgetAccessRightService
    {
        private readonly IBudgetsProvider _db;
        private readonly BudgetAccessRightValidator _validator;

        public BudgetAccessRightService(IBudgetsProvider db)
        {
            this._db = db;
            this._validator = new BudgetAccessRightValidator();
        }

        /// <summary>
        ///     Validates budget access rights
        /// </summary>
        /// <param name="budgetAccessRight">Budget access right to validate</param>
        /// <returns>True if budget access right is valid, false otherwise</returns>
        public void Validate(BudgetAccessRight budgetAccessRight)
        {
            if (budgetAccessRight == null)
                throw new ArgumentNullException(nameof(budgetAccessRight));

            this._validator.ValidateAndThrowCustomException(budgetAccessRight);
        }

        /// <summary>
        ///     Returns access rights for specified budget
        /// </summary>
        /// <param name="id">ID of budget</param>
        /// <returns>Access rights for budget</returns>
        public IQueryable GetAccessRightsByBudgetId(Guid id)
        {
            return this._db.BudgetAccessRights.Where(right => right.Budget.Guid.Equals(id));
        }

        /// <summary>
        ///     Returns users IDs for specified budget
        /// </summary>
        /// <param name="id">ID of budget</param>
        /// <returns>List of user IDs</returns>
        public IList<Guid> GetUsersGuidListForBudget(Guid id)
        {
            var accessRights = this.GetBudgetAccessRightsByBudget(id);

            return accessRights.Select(item => item.UserProfile.Guid).ToList();
        }

        /// <summary>
        ///     Returns budget by it's ID
        /// </summary>
        /// <param name="id">ID of budget</param>
        /// <returns>Desired budget</returns>
        private async Task<Budget> GetBudgetById(Guid id)
        {
            return await this._db.Budgets.Where(budget => budget.Guid.Equals(id)).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Returns access rights creator by creator's ID
        /// </summary>
        /// <param name="id">ID of a creator</param>
        /// <returns>Access rights creator</returns>
        private UserProfile GetAccessRightsCreator(Guid id)
        {
            return this._db.UserProfiles.FirstOrDefault(user => user.Guid == id);
        }

        /// <summary>
        ///     Creates new access budget right
        /// </summary>
        /// <param name="budgetId">ID of a budget</param>
        /// <param name="assignedUserId">Assigned user's ID</param>
        /// <param name="permission">Permission for right</param>
        /// <returns>False if user has already accessright to this budget or when adding failed. True otherwise.</returns>
        public async Task<bool> CreateBudgetAccessRight(Guid budgetId, Guid assignedUserId, PermissionEnum permission)
        {
            // create budget access right only if it isn't already present
            bool budgetAccessRightExists = await _db.BudgetAccessRights.Where(b => b.Budget.Guid == budgetId && b.UserProfile.Guid == assignedUserId).CountAsync() > 0;
            if (budgetAccessRightExists)
            {
                return false;
            }

            // find budget by its Id
            var budget = await this.GetBudgetById(budgetId);
            // finding creator by his ID
            var assignedUser = this.GetAccessRightsCreator(assignedUserId);

            var budgetAccessRight = new BudgetAccessRight
            {
                Budget = budget,
                Permission = permission,
                UserProfile = assignedUser
            };

            this.Validate(budgetAccessRight);
            
            // creating new budget access right
            return await this._db.AddOrUpdateAsync(budgetAccessRight);
        }

        /// <summary>
        ///     Returns budget access right by specified ID
        /// </summary>
        /// <param name="id">Access right ID</param>
        /// <returns>Desired budget access right</returns>
        public async Task<BudgetAccessRight> GetBudgetAccessRightById(Guid id)
        {
            return await this._db.BudgetAccessRights.Where(right => right.Guid.Equals(id)).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Edits budget access right
        /// </summary>
        /// <param name="budgetAccessRightId">ID of changed access right</param>
        /// <param name="permission">New permission</param>
        /// <param name="userProfileId">New ID of a user</param>
        /// <returns></returns>
        public async Task EditBudgetAccessRight(Guid budgetAccessRightId, PermissionEnum permission, Guid userProfileId)
        {
            var accessRightToEdit = await this.GetBudgetAccessRightById(budgetAccessRightId);

            accessRightToEdit.Permission = permission;
            accessRightToEdit.UserProfile = await this.GetUserProfileById(userProfileId);

            this.Validate(accessRightToEdit);

            await this._db.AddOrUpdateAsync(accessRightToEdit);
        }

        /// <summary>
        ///     Returns user profile by it's ID
        /// </summary>
        /// <param name="id">ID of user profile</param>
        /// <returns>Desired user profile</returns>
        public async Task<UserProfile> GetUserProfileById(Guid id)
        {
            return await this._db.UserProfiles.Where(profile => profile.Guid.Equals(id)).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Deletes budget access right
        /// </summary>
        /// <param name="id">ID of deleted access right</param>
        /// <returns></returns>
        public async Task DeleteBudgetAccessRight(Guid id)
        {
            var budgetAccessRight = await this.GetBudgetAccessRightById(id);
            await this._db.DeteleAsync(budgetAccessRight);
        }

        /// <summary>
        ///     Returns all user profiles
        /// </summary>
        /// <param name="users">ID of users</param>
        /// <param name="currentUserId">ID of current user</param>
        /// <returns>All user profiles</returns>
        public IQueryable<UserProfile> GetUserProfiles(ICollection<Guid> users, Guid currentUserId)
        {
            return this._db.UserProfiles
                .Where(
                    profile => // filtering users which don't have owner permission or other permissions
                        profile.BudgetAccessRights.All(right => !users.Contains(profile.Guid)));
        }

        #region private

        private ICollection<BudgetAccessRight> GetBudgetAccessRightsByBudget(Guid id)
        {
            return this._db.Budgets.FirstOrDefault(budget => budget.Guid.Equals(id)).AccessRights;
        }

        #endregion
    }
}