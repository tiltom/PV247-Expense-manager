using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Entity.Users;

namespace ExpenseManager.BusinessLogic.Budget
{
    /// <summary>
    ///     Class that handles logic of BudgetAccessRightController
    /// </summary>
    public class BudgetAccessRightService
    {
        private readonly IBudgetsProvider _db = ProvidersFactory.GetNewBudgetsProviders();

        public IQueryable GetAccessRightsByBudgetId(Guid id)
        {
            return this._db.BudgetAccessRights.Where(right => right.Budget.Guid.Equals(id));
        }

        public IList<Guid> GetUsersGuidListForBudget(Guid id)
        {
            var accessRights = this.GetBudgetAccessRightsByBudget(id);

            return accessRights.Select(item => item.UserProfile.Guid).ToList();
        }

        private async Task<Budget> GetBudgetById(Guid id)
        {
            return await this._db.Budgets.Where(b => b.Guid.Equals(id)).FirstOrDefaultAsync();
        }

        private UserProfile GetAccessRightsCreator(string id)
        {
            return this._db.UserProfiles.FirstOrDefault(user => user.Guid.ToString() == id);
        }

        public async Task CreateAccessBudgetRight(Guid budgetId, string assignedUserId, PermissionEnum permission)
        {
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

            this.ValidateBudgetAccessRight(budgetAccessRight);

            // creating new budget access right
            await this._db.AddOrUpdateAsync(budgetAccessRight);
        }

        public async Task<BudgetAccessRight> GetBudgetAccessRightById(Guid id)
        {
            return await this._db.BudgetAccessRights.Where(x => x.Guid.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task EditBudgetAccessRight(BudgetAccessRight accessRight)
        {
            var accessRightToEdit = await this.GetBudgetAccessRightById(accessRight.Guid);

            accessRightToEdit.Budget = accessRight.Budget;
            accessRightToEdit.Permission = accessRight.Permission;
            accessRightToEdit.UserProfile = accessRight.UserProfile;

            this.ValidateBudgetAccessRight(accessRightToEdit);

            await this._db.AddOrUpdateAsync(accessRightToEdit);
        }

        public async Task DeleteBudgetAccessRight(Guid id)
        {
            var budgetAccessRight = await this.GetBudgetAccessRightById(id);
            // TODO: add check for permissions
            await this._db.DeteleAsync(budgetAccessRight);
        }

        public IQueryable<UserProfile> GetUserProfiles(ICollection<Guid> users, Guid currentUserId)
        {
            return this._db.UserProfiles
                .Where(
                    u => // filtering users which don't have owner permission or other permissions
                        u.BudgetAccessRights.All(war => war.Budget.Creator.Guid != currentUserId) ||
                        !users.Contains(u.Guid));
        }

        private ICollection<BudgetAccessRight> GetBudgetAccessRightsByBudget(Guid id)
        {
            return this._db.Budgets.FirstOrDefault(x => x.Guid.Equals(id)).AccessRights;
        }

        private void ValidateBudgetAccessRight(BudgetAccessRight budgetAccessRight)
        {
            // TODO: add validation
        }
    }
}