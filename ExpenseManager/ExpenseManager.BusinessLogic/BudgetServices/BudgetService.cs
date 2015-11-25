using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Users;

namespace ExpenseManager.BusinessLogic.BudgetServices
{
    /// <summary>
    ///     Class that handles logic of BudgetController
    /// </summary>
    public class BudgetService
    {
        private readonly IBudgetsProvider _db;
        private readonly ITransactionsProvider _transactionsProvider;

        public BudgetService(IBudgetsProvider db, ITransactionsProvider transactionsProvider)
        {
            this._db = db;
            this._transactionsProvider = transactionsProvider;
        }

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
        public async Task CreateBudget(Budget budget)
        {
            if (this.ValidateBudget(budget))
            {
                await this._db.AddOrUpdateAsync(budget);
            }
        }

        /// <summary>
        ///     Returns budget by specified guid
        /// </summary>
        /// <param name="guid">ID that specifies returned budget</param>
        /// <returns>Desired budget</returns>
        public async Task<Budget> GetBudgetById(Guid guid)
        {
            return await this._db.Budgets.Where(x => x.Guid.Equals(guid)).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Edits budget and saves it to database
        /// </summary>
        /// <param name="budgetId">ID of edited budget</param>
        /// <param name="name">New name of budget</param>
        /// <param name="description">New description of budget</param>
        /// <param name="limit">New limit of budget</param>
        /// <param name="startDate">New start date of budget</param>
        /// <param name="endDate">New end date of budget</param>
        /// <returns></returns>
        public async Task EditBudget(Guid budgetId, string name, string description, decimal limit,
            DateTime startDate, DateTime endDate)
        {
            var originalBudget = await this.GetBudgetById(budgetId);

            originalBudget.Name = name;
            originalBudget.StartDate = startDate;
            originalBudget.EndDate = endDate;
            originalBudget.Description = description;
            originalBudget.Limit = limit;

            if (this.ValidateBudget(originalBudget))
            {
                await this._db.AddOrUpdateAsync(originalBudget);
            }
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

        /// <summary>
        ///     Validates budget
        /// </summary>
        /// <param name="budget">Budget to validate</param>
        /// <returns>True if budget is valid, false otherwise</returns>
        public bool ValidateBudget(Budget budget)
        {
            if (budget == null)
            {
                return false;
            }

            if (budget.Currency == null)
            {
                return false;
            }

            if (budget.Creator == null)
            {
                return false;
            }

            if (budget.Name.Equals(string.Empty))
            {
                return false;
            }

            if (!ValidateModel(budget.StartDate, budget.EndDate))
            {
                return false;
            }

            if (budget.Limit <= 0)
            {
                return false;
            }

            return true;
        }
    }
}