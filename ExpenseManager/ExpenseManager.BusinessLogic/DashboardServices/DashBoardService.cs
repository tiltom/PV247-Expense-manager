using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExpenseManager.BusinessLogic.DashboardServices.Models;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Transactions;

namespace ExpenseManager.BusinessLogic.DashboardServices
{
    public class DashBoardService
    {
        private const int WeekInterval = 60;
        private const int MonthsInterval = 600;
        private const int DaysInterval = 10;
        private readonly ITransactionsProvider _transactionsProvider;

        public DashBoardService(ITransactionsProvider transactionsProvider)
        {
            this._transactionsProvider = transactionsProvider;
        }

        /// <summary>
        ///     Will return just transaction which can be seen by this user
        /// </summary>
        /// <param name="userId"> id of logged user</param>
        /// <returns> queryable for accessible transactions</returns>
        public IQueryable<Transaction> GetAccessibleResults(Guid userId)
        {
            return
                this._transactionsProvider.Transactions
                    .Where(t =>
                        (t.Wallet.WalletAccessRights.Any(war => war.UserProfile.Guid == userId) ||
                         t.Budget.AccessRights.Any(bar => bar.UserProfile.Guid == userId))
                    );
        }

        /// <summary>
        ///     provides list of wallets available for user
        /// </summary>
        /// <param name="userId"> user with access to provided wallets</param>
        /// <returns>select item for front end</returns>
        public async Task<List<SelectListItem>> GetAccessibleWallets(Guid userId)
        {
            return await
                this._transactionsProvider.Wallets.Where(
                    w => w.WalletAccessRights.Any(war => war.UserProfile.Guid == userId)).Select(w => new SelectListItem
                    {
                        Value = w.Guid.ToString(),
                        Text = w.Name
                    }).ToListAsync();
        }

        /// <summary>
        ///     Generate wrapper values using categories
        /// </summary>
        /// <param name="collection"></param>
        /// <returns> non negative values for all categories</returns>
        public async Task<List<SimpleGraphModel>> GetWrapperValuesForCategories(IQueryable<Transaction> collection)
        {
            return await collection
                .GroupBy(t => t.Category.Name)
                .Select(
                    x =>
                        new SimpleGraphModel
                        {
                            Label = x.Key,
                            Value = x.Sum(f => Math.Abs(f.Amount))
                        })
                .ToListAsync();
        }

        /// <summary>
        ///     Returns graph data for months in last year
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public async Task<GraphWithDescriptionModel> GetGraphForMonthLastYear(IQueryable<Transaction> collection)
        {
            var result = await
                collection.GroupBy(t => new {t.Date.Month, t.Date.Year}).Select(
                    x =>
                        new SimpleGraphModel
                        {
                            Label = x.Key.Month + ". " + x.Key.Year,
                            Value = x.Sum(f => f.Amount)
                        })
                    .ToListAsync();
            return new GraphWithDescriptionModel {Description = "Last year report", GraphData = result};
        }

        /// <summary>
        ///     Returns graph data for days in last month
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public async Task<GraphWithDescriptionModel> GetGraphForDaysLastMonth(IQueryable<Transaction> collection)
        {
            var result = await
                collection.GroupBy(t => new {t.Date.Day, t.Date.Month}).Select(
                    x =>
                        new SimpleGraphModel
                        {
                            Label = x.Key.Day + ". " + x.Key.Month + ".",
                            Value = x.Sum(f => f.Amount)
                        })
                    .ToListAsync();
            return new GraphWithDescriptionModel {Description = "Last Month Report", GraphData = result};
        }

        /// <summary>
        ///     Filter transactions using filter
        /// </summary>
        /// <param name="filter"> filter to be used</param>
        /// <param name="currentUser">guid of logged user profile</param>
        /// <returns></returns>
        public IQueryable<Transaction> FilterTransactions(FilterDataServiceModel filter, Guid currentUser)
        {
            return this.GetAccessibleResults(currentUser)
                .Where(
                    t =>
                        t.Date <= filter.EndDate && t.Date >= filter.StartDate
                        && (!filter.Budgets.Any() || filter.Budgets.Contains(t.Budget.Guid))
                        && (!filter.Wallets.Any() || filter.Wallets.Contains(t.Wallet.Guid))
                        && (!filter.Categories.Any() || filter.Categories.Contains(t.Category.Guid))
                );
        }
    }
}