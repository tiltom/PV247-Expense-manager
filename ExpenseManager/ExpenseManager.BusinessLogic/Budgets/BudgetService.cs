using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Entity.Users;

namespace ExpenseManager.BusinessLogic.Budgets
{
    /// <summary>
    ///     Class that handles logic of BudgetController
    /// </summary>
    public class BudgetService
    {
        private readonly IBudgetsProvider _db = ProvidersFactory.GetNewBudgetsProviders();
        private readonly ITransactionsProvider _transactionsProvider = ProvidersFactory.GetNewTransactionsProviders();

        /// <summary>
        ///     Additional validation for model
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static bool ValidateModel(DateTime startDate, DateTime endDate)
        {
            return startDate <= endDate;
        }

        /// <summary>
        ///     Returns all budgets that belongs to specified user
        /// </summary>
        /// <param name="userId">User that owns budgets</param>
        /// <returns>All budgets for user</returns>
        public IQueryable GetBudgetsByUserId(Guid userId)
        {
            return this._db.Budgets.Where(user => user.Creator.Guid.Equals(userId));
        }

        /// <summary>
        ///     Returns owner (creator) of the budget
        /// </summary>
        /// <param name="userId">ID of budget creator</param>
        /// <returns>Budget creator</returns>
        public async Task<UserProfile> GetBudgetCreator(Guid userId)
        {
            return await this._db.UserProfiles.FirstOrDefaultAsync(user => user.Guid.Equals(userId));
        }

        /// <summary>
        ///     Creates new budget - adds it to database
        /// </summary>
        /// <param name="budget">New budget</param>
        /// <returns></returns>
        public async Task CreateBudget(Entity.Budgets.Budget budget)
        {
            this.ValidateBudget(budget);

            await this._db.AddOrUpdateAsync(budget);
        }

        /// <summary>
        ///     Returns budget by specified guid
        /// </summary>
        /// <param name="guid">ID that specifies returned budget</param>
        /// <returns>Desired budget</returns>
        public async Task<Entity.Budgets.Budget> GetBudgetById(Guid guid)
        {
            return await this._db.Budgets.Where(x => x.Guid.Equals(guid)).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Edits budget and saves it to database
        /// </summary>
        /// <param name="editedBudget">Edited budget</param>
        /// <returns></returns>
        public async Task EditBudget(Entity.Budgets.Budget editedBudget)
        {
            var originalBudget = await this.GetBudgetById(editedBudget.Guid);

            originalBudget.Name = editedBudget.Name;
            originalBudget.StartDate = editedBudget.StartDate;
            originalBudget.EndDate = editedBudget.EndDate;
            originalBudget.Description = editedBudget.Description;
            originalBudget.Limit = editedBudget.Limit;

            this.ValidateBudget(originalBudget);

            await this._db.AddOrUpdateAsync(originalBudget);
        }

        /// <summary>
        ///     Deletes budget, it's access rights and transactions
        /// </summary>
        /// <param name="guid">ID of deleted budget</param>
        /// <returns></returns>
        public async Task DeleteBudget(Guid guid)
        {
            var budget = await this._db.Budgets.Where(x => x.Guid.Equals(guid)).FirstOrDefaultAsync();

            // delete budget rights
            foreach (var right in budget.AccessRights)
            {
                await this._db.DeteleAsync(right);
            }

            // delete transactions
            foreach (var transaction in budget.Transactions)
            {
                await this._transactionsProvider.DeteleAsync(transaction);
            }

            // delete budget itself
            await this._db.DeteleAsync(budget);
        }

        #region private

        private void ValidateBudget(Entity.Budgets.Budget budget)
        {
            // TODO: validate budget
        }

        #endregion
    }
}