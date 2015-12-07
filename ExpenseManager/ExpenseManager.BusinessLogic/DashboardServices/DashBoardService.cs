﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Chart.Mvc.ComplexChart;
using Chart.Mvc.SimpleChart;
using ExpenseManager.BusinessLogic.DashboardServices.Models;
using ExpenseManager.BusinessLogic.ExchangeRates;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Queryable;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Resources.DashboardResources;

namespace ExpenseManager.BusinessLogic.DashboardServices
{
    public class DashBoardService : ServiceWithWallet
    {
        public const int NumberOfTransactionsOnDashBoard = 5;
        private readonly ColorGeneratorService _colorGenerator;
        private readonly ITransactionsProvider _transactionsProvider;

        /// <summary>
        ///     Constructor with services and providers required by this service
        /// </summary>
        /// <param name="transactionsProvider"> provider with db access to transactions</param>
        /// <param name="colorGeneratorService">service which can generate colors</param>
        public DashBoardService(ITransactionsProvider transactionsProvider, ColorGeneratorService colorGeneratorService)
        {
            this._transactionsProvider = transactionsProvider;
            this._colorGenerator = colorGeneratorService;
        }

        #region protected

        protected override IWalletsQueryable WalletsProvider
        {
            get { return this._transactionsProvider; }
        }

        #endregion

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
        /// <param name="collection"> transaction collection</param>
        /// <param name="currency"> currency of user wallet</param>
        /// <returns> non negative values for all categories</returns>
        public async Task<List<SimpleGraphModel>> GetWrapperValuesForCategories(
            IQueryable<Transaction> collection,
            Currency currency)
        {
            var data = await collection.ToListAsync();
            return data.Select(t => Transformation.ChangeCurrencyForNewTransaction(t, currency))
                .GroupBy(t => t.Category.Name)
                .Select(
                    x =>
                        new SimpleGraphModel
                        {
                            Label = x.Key,
                            Value = x.Sum(f => Math.Abs(f.Amount))
                        })
                .ToList();
        }

        /// <summary>
        ///     Returns graph data for months in last year
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="currency"> currency of user wallet</param>
        /// <returns>Graph data for every month in last year</returns>
        public async Task<GraphWithDescriptionModel> GetGraphForMonthLastYear(
            IQueryable<Transaction> collection,
            Currency currency)
        {
            var data = await collection.ToListAsync();
            var result = data.Select(t => Transformation.ChangeCurrencyForNewTransaction(t, currency))
                .GroupBy(t => new {t.Date.Month, t.Date.Year}).Select(
                    x =>
                        new SimpleGraphModel
                        {
                            Label = x.Key.Month + DashBoardResource.MonthDelimiter + x.Key.Year,
                            Value = x.Sum(f => f.Amount)
                        })
                .ToList();
            return new GraphWithDescriptionModel {Description = DashBoardResource.LastYearReport, GraphData = result};
        }

        /// <summary>
        ///     Returns graph data for days in last month
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="currency"> currency of user wallet</param>
        /// <returns> graph for every day for last month</returns>
        public async Task<GraphWithDescriptionModel> GetGraphForDaysLastMonth(
            IQueryable<Transaction> collection,
            Currency currency)
        {
            var data = await collection.ToListAsync();
            var result =
                data.Select(t => Transformation.ChangeCurrencyForNewTransaction(t, currency))
                    .GroupBy(t => new {t.Date.Day, t.Date.Month})
                    .Select(
                        x =>
                            new SimpleGraphModel
                            {
                                Label = x.Key.Day + DashBoardResource.MonthDelimiter + x.Key.Month,
                                Value = x.Sum(f => f.Amount)
                            })
                    .ToList();
            return new GraphWithDescriptionModel {Description = DashBoardResource.LastMonthReport, GraphData = result};
        }

        /// <summary>
        ///     Filter transactions using filter
        /// </summary>
        /// <param name="mappedFilter"> filter to be used</param>
        /// <param name="userId">guid of logged user profile</param>
        /// <returns>DashBoardServiceModel filled with graphs transactions and other data </returns>
        public async Task<DashBoardServiceModel> GenerateDataForFilter(FilterDataServiceModel mappedFilter, Guid userId)
        {
            // select transactions according to filter
            var userWallet = await this.GetWalletByUserId(userId);
            var resultMonth = this.FilterTransactions(mappedFilter.WithMonthFilterValues(), userId);
            var resultYear = this.FilterTransactions(mappedFilter.WithYearFilterValues(), userId);
            // prepare data for graphs
            var categoriesExpense =
                await this.GetWrapperValuesForCategories(resultMonth.Where(t => t.Amount < 0), userWallet.Currency);
            var categoriesIncome =
                await this.GetWrapperValuesForCategories(resultMonth.Where(t => t.Amount >= 0), userWallet.Currency);
            var monthSummary = await this.GetGraphForDaysLastMonth(resultMonth, userWallet.Currency);
            var yearSummary = await this.GetGraphForMonthLastYear(resultYear, userWallet.Currency);
            // generate charts
            return new DashBoardServiceModel
            {
                CategoriesExpenseChart = this.GeneratePieChart(categoriesExpense),
                CategoriesIncomeChart = this.GeneratePieChart(categoriesIncome),
                MonthSummaryChart = this.GenerateLineChart(monthSummary),
                YearSummaryChart = this.GenerateLineChart(yearSummary),
                Transactions = await this.LastTransactions(userId)
            };
        }

        #region private

        /// <summary>
        ///     Filter transactions using filter
        /// </summary>
        /// <param name="filter"> filter to be used</param>
        /// <param name="currentUser">guid of logged user profile</param>
        /// <returns>Transaction filling criteria in filter</returns>
        private IQueryable<Transaction> FilterTransactions(
            FilterDataServiceModel filter,
            Guid currentUser)
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

        private async Task<List<Transaction>> LastTransactions(Guid userId)
        {
            var lastTransactions =
                await
                    this.GetAccessibleResults(userId)
                        .OrderByDescending(t => t.Date)
                        .Take(NumberOfTransactionsOnDashBoard)
                        .ToListAsync();
            return lastTransactions;
        }


        private LineChart GenerateLineChart(GraphWithDescriptionModel data)
        {
            // check if  enough data for line chart (with one point the chart is broken)
            if (data.GraphData.Count < 2)
            {
                return null;
            }
            var lineChart = new LineChart
            {
                ComplexData =
                {
                    Labels = data.GraphData.Select(t => t.Label).ToList(),
                    Datasets = new List<ComplexDataset>
                    {
                        new ComplexDataset
                        {
                            Data = data.GraphData.Select(t => Convert.ToDouble(t.Value)).ToList(),
                            Label = data.Description,
                            FillColor = ColorGeneratorService.Transparent,
                            StrokeColor = this._colorGenerator.GenerateColor(),
                            PointColor = ColorGeneratorService.Black,
                            PointStrokeColor = ColorGeneratorService.White,
                            PointHighlightFill = ColorGeneratorService.White,
                            PointHighlightStroke = ColorGeneratorService.Black
                        }
                    }
                }
            };
            // graph settings for dynamic charts
            lineChart.ChartConfiguration.ScaleBeginAtZero = false;
            lineChart.ChartConfiguration.Responsive = true;
            return lineChart;
        }

        private PieChart GeneratePieChart(List<SimpleGraphModel> result)
        {
            // check if enough data for displaying pie chart
            if (result.Count == 0)
            {
                return null;
            }
            var sum = result.Sum(t => t.Value);
            return new PieChart
            {
                Data = result.Select(
                    t =>
                        new SimpleData
                        {
                            Label = t.Label,
                            // compute part in percents and with two decimal
                            Value = Math.Round(Math.Abs(Convert.ToDouble(100*t.Value/sum)), 2),
                            Color = this._colorGenerator.GenerateColor()
                        }).ToList()
            };
        }

        #endregion
    }
}