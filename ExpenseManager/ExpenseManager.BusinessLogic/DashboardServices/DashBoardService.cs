using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Chart.Mvc.ComplexChart;
using Chart.Mvc.SimpleChart;
using ExpenseManager.BusinessLogic.DashboardServices.Models;
using ExpenseManager.BusinessLogic.ExchangeRates;
using ExpenseManager.BusinessLogic.ServicesConstants;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Budgets;
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
        ///     provides list of wallets available for user
        /// </summary>
        /// <param name="userId"> user with access to provided wallets</param>
        /// <returns>select item for front end</returns>
        public async Task<List<SelectListItem>> GetAccessibleWallets(Guid userId)
        {
            return await
                this._transactionsProvider.Wallets.Where(
                    w => w.WalletAccessRights.Any(right => right.UserProfile.Guid == userId))
                    .Select(wallet => new SelectListItem
                    {
                        Value = wallet.Guid.ToString(),
                        Text = wallet.Name
                    }).ToListAsync();
        }

        /// <summary>
        ///     Generate wrapper values using categories
        /// </summary>
        /// <param name="collection"> transaction collection</param>
        /// <param name="currency"> currency of user wallet</param>
        /// <returns> non negative values for all categories</returns>
        internal async Task<List<SimpleGraphModel>> GetWrapperValuesForCategories(
            IQueryable<Transaction> collection,
            Currency currency)
        {
            var data = await collection.ToListAsync();
            return data.Select(transaction => Transformation.ChangeCurrencyForNewTransaction(transaction, currency))
                .GroupBy(transaction => transaction.Category.Name)
                .Select(
                    group =>
                        new SimpleGraphModel
                        {
                            Label = group.Key,
                            Value = group.Sum(transaction => Math.Abs(transaction.Amount))
                        })
                .ToList();
        }

        /// <summary>
        ///     Returns graph data for months in last year
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="currency"> currency of user wallet</param>
        /// <returns>Graph data for every month in last year</returns>
        private async Task<GraphWithDescriptionModel> GetGraphForMonthLastYear(
            IQueryable<Transaction> collection,
            Currency currency)
        {
            var data = await collection.ToListAsync();
            var result =
                data.Select(transaction => Transformation.ChangeCurrencyForNewTransaction(transaction, currency))
                    .GroupBy(transaction => new {transaction.Date.Month, transaction.Date.Year}).Select(
                        group =>
                            new SimpleGraphModel
                            {
                                Label = group.Key.Month + DashBoardResource.MonthDelimiter + group.Key.Year,
                                Value = group.Sum(transaction => transaction.Amount)
                            })
                    .ToList();
            return new GraphWithDescriptionModel
            {
                Description = DashBoardResource.LastYearTabDescription,
                GraphData = result
            };
        }

        /// <summary>
        ///     Returns graph data for days in last month
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="currency"> currency of user wallet</param>
        /// <returns> graph for every day for last month</returns>
        internal async Task<GraphWithDescriptionModel> GetGraphForDaysLastMonth(
            IQueryable<Transaction> collection,
            Currency currency)
        {
            var data = await collection.ToListAsync();
            var result =
                data.Select(transaction => Transformation.ChangeCurrencyForNewTransaction(transaction, currency))
                    .GroupBy(transaction => new {transaction.Date.Day, transaction.Date.Month})
                    .Select(
                        group =>
                            new SimpleGraphModel
                            {
                                Label = group.Key.Day + DashBoardResource.MonthDelimiter + group.Key.Month,
                                Value = group.Sum(transaction => transaction.Amount)
                            })
                    .ToList();
            return new GraphWithDescriptionModel
            {
                Description = DashBoardResource.LastMonthTabDescription,
                GraphData = result
            };
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
                await
                    this.GetWrapperValuesForCategories(resultMonth.Where(transaction => transaction.Amount < 0),
                        userWallet.Currency);
            var categoriesIncome =
                await
                    this.GetWrapperValuesForCategories(resultMonth.Where(transaction => transaction.Amount >= 0),
                        userWallet.Currency);
            var monthSummary = await this.GetGraphForDaysLastMonth(resultMonth, userWallet.Currency);
            var yearSummary = await this.GetGraphForMonthLastYear(resultYear, userWallet.Currency);
            // generate charts
            return new DashBoardServiceModel
            {
                CategoriesExpenseChart = this.GeneratePieChart(categoriesExpense),
                CategoriesIncomeChart = this.GeneratePieChart(categoriesIncome),
                MonthSummaryChart = this.GenerateLineChart(monthSummary),
                YearSummaryChart = this.GenerateLineChart(yearSummary),
                Transactions = await this.LastTransactions(resultYear),
                BudgetLimitChart = await this.CreateChartForBudgetLimits(userId, userWallet.Currency)
            };
        }

        #region private

        private async Task<BarChart> CreateChartForBudgetLimits(Guid userId, Currency currency)
        {
            var budgets = await this.GetAccessibleBudgets(userId);
            var transactionFromBudgets = await this.GetTransactionFromBudgets(budgets);
            var graphModels = new List<BudgetWithLimitGraphModel>();
            var complexGraphModel = new BudgetLimitGraphModel
            {
                Values = graphModels,
                Label = DashBoardResource.BudgetTabDescription
            };

            graphModels.AddRange(from budget in budgets
                let budgetTransactions =
                    transactionFromBudgets.ContainsKey(budget.Guid.ToString())
                        ? transactionFromBudgets[budget.Guid.ToString()]
                        : Enumerable.Empty<Transaction>()
                let sumInUserCurrency =
                    budgetTransactions.Select(
                        transaction => Transformation.ChangeCurrencyForNewTransaction(transaction, currency))
                        .Sum(transaction => transaction.Amount)
                select new BudgetWithLimitGraphModel
                {
                    Label = budget.Name,
                    BudgetLimit = budget.Limit,
                    ComputedTransaction = sumInUserCurrency*-1
                });
            return this.GenerateBarChart(complexGraphModel);
        }

        private async Task<List<Budget>> GetAccessibleBudgets(Guid userId)
        {
            var budgets = await this._transactionsProvider.Budgets.Where(
                budget =>
                    budget.AccessRights.Any(
                        right => right.UserProfile.Guid == userId && right.Permission == PermissionEnum.Owner))
                .ToListAsync();
            return budgets;
        }

        private async Task<Dictionary<string, IEnumerable<Transaction>>> GetTransactionFromBudgets(List<Budget> budgets)
        {
            var budgetIds = budgets.Select(budget => budget.Guid.ToString());
            return await this._transactionsProvider.Transactions
                .Where(transaction => budgetIds.Any(budgetId => transaction.Budget.Guid.ToString() == budgetId))
                .GroupBy(transaction => transaction.Budget.Guid.ToString())
                .ToDictionaryAsync(group => @group.Key, group => @group.Select(t => t));
        }


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
                    transaction =>
                        transaction.Date <= filter.EndDate && transaction.Date >= filter.StartDate
                        && (!filter.Budgets.Any() || filter.Budgets.Contains(transaction.Budget.Guid))
                        && (!filter.Wallets.Any() || filter.Wallets.Contains(transaction.Wallet.Guid))
                        && (!filter.Categories.Any() || filter.Categories.Contains(transaction.Category.Guid))
                ).OrderBy(transaction => transaction.Date);
        }

        private async Task<List<Transaction>> LastTransactions(IQueryable<Transaction> transactions)
        {
            return
                await
                    transactions
                        .OrderByDescending(transaction => transaction.Date)
                        .Take(NumberOfTransactionsOnDashBoard)
                        .ToListAsync();
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
                    Labels = data.GraphData.Select(graphData => graphData.Label).ToList(),
                    Datasets = this.GenerateDataSets(data)
                }
            };
            // graph settings for dynamic charts
            lineChart.ChartConfiguration.ScaleBeginAtZero = false;
            lineChart.ChartConfiguration.Responsive = true;
            lineChart.ChartConfiguration.BezierCurve = false;
            return lineChart;
        }

        private List<ComplexDataset> GenerateDataSets(GraphWithDescriptionModel data)
        {
            return new List<ComplexDataset>
            {
                new ComplexDataset
                {
                    Data = data.GraphData.Select(t => Convert.ToDouble(t.Value)).ToList(),
                    Label = data.Description,
                    FillColor = ColorGeneratorConstants.Transparent,
                    StrokeColor = this._colorGenerator.GenerateColor(),
                    PointColor = ColorGeneratorConstants.Black,
                    PointStrokeColor = ColorGeneratorConstants.White,
                    PointHighlightFill = ColorGeneratorConstants.White,
                    PointHighlightStroke = ColorGeneratorConstants.Black
                }
            };
        }


        private BarChart GenerateBarChart(BudgetLimitGraphModel data)
        {
            // check if  enough data for bar chart
            if (!data.Values.Any())
            {
                return null;
            }
            var barChart = new BarChart
            {
                ComplexData =
                {
                    Labels = data.Values.Select(graphData => graphData.Label).ToList(),
                    Datasets =
                        new List<ComplexDataset>
                        {
                            this.GenerateComplexData(data.Values.Select(t => t.BudgetLimit).ToList(),
                                ColorGeneratorConstants.Grey),
                            this.GenerateComplexData(data.Values.Select(t => t.ComputedTransaction).ToList(),
                                ColorGeneratorConstants.Red)
                        }
                }
            };
            // graph settings for dynamic charts
            barChart.ChartConfiguration.ScaleBeginAtZero = false;
            barChart.ChartConfiguration.Responsive = true;
            return barChart;
        }

        private ComplexDataset GenerateComplexData(List<decimal> values, string color)
        {
            return new ComplexDataset
            {
                Data = values.Select(Convert.ToDouble).ToList(),
                FillColor = color,
                StrokeColor = color,
                PointColor = ColorGeneratorConstants.Black,
                PointStrokeColor = ColorGeneratorConstants.White,
                PointHighlightFill = ColorGeneratorConstants.White,
                PointHighlightStroke = ColorGeneratorConstants.Black
            };
        }

        private PieChart GeneratePieChart(List<SimpleGraphModel> result)
        {
            // check if enough data for displaying pie chart
            if (result.Count == 0)
            {
                return null;
            }
            var sum = result.Sum(model => model.Value);
            return new PieChart
            {
                Data = result.Select(
                    model =>
                        new SimpleData
                        {
                            Label = model.Label,
                            // compute part in percents and with two decimal
                            Value = Math.Round(Math.Abs(Convert.ToDouble(100*model.Value/sum)), 2),
                            Color = this._colorGenerator.GenerateColor()
                        }).ToList()
            };
        }


        /// <summary>
        ///     Will return just transaction which can be seen by this user
        /// </summary>
        /// <param name="userId"> id of logged user</param>
        /// <returns> queryable for accessible transactions</returns>
        private IQueryable<Transaction> GetAccessibleResults(Guid userId)
        {
            return
                this._transactionsProvider.Transactions
                    .Where(transaction =>
                        transaction.Wallet.WalletAccessRights.Any(right => right.UserProfile.Guid == userId) ||
                        transaction.Budget.AccessRights.Any(right => right.UserProfile.Guid == userId)
                    );
        }

        #endregion
    }
}