using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Chart.Mvc.ComplexChart;
using Chart.Mvc.SimpleChart;
using ExpenseManager.BusinessLogic.CommonServices;
using ExpenseManager.BusinessLogic.DashboardServices;
using ExpenseManager.BusinessLogic.DashboardServices.Models;
using ExpenseManager.Database;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Web.Models.HomePage;
using ExpenseManager.Web.Models.Transaction;

namespace ExpenseManager.Web.Controllers
{
    [RequireHttps]
    public class HomeController : AbstractController
    {
        private readonly ColorGeneratorService _colorGenerator = new ColorGeneratorService();

        private readonly DashBoardService _dashBoardService =
            new DashBoardService(ProvidersFactory.GetNewTransactionsProviders());

        /// <summary>
        ///     Will display empty page with initialized filter
        /// </summary>
        /// <returns>Initialized filter</returns>
        public async Task<ViewResult> Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var filter = new FilterDataModel();
                var mappedFilter = Mapper.Map<FilterDataServiceModel>(filter);
                var userId = await this.CurrentProfileId();
                // select transactions according to filter
                var resultMonth = this._dashBoardService.FilterTransactions(mappedFilter.WithMonthFilterValues(), userId);
                var resultYear = this._dashBoardService.FilterTransactions(mappedFilter.WithYearFilterValues(), userId);
                var last5Transactions =
                    await
                        this._dashBoardService.GetAccessibleResults(userId)
                            .OrderByDescending(t => t.Date)
                            .Take(5).ProjectTo<TransactionShowModel>()
                            .ToListAsync();
                // prepare data for graphs
                var categoriesExpense =
                    await this._dashBoardService.GetWrapperValuesForCategories(resultMonth.Where(t => t.Amount < 0));
                var categoriesIncome =
                    await this._dashBoardService.GetWrapperValuesForCategories(resultMonth.Where(t => t.Amount >= 0));
                var monthSummary = await this._dashBoardService.GetGraphForDaysLastMonth(resultMonth);
                var yearSummary = await this._dashBoardService.GetGraphForMonthLastYear(resultYear);
                // generate pie chart
                var categoriesExpenseChart = this.GeneratePieChart(categoriesExpense);
                var categoriesIncomeChart = this.GeneratePieChart(categoriesIncome);
                var monthSummaryChart = this.GenerateBarChart(monthSummary);
                var yearSummaryChart = this.GenerateBarChart(yearSummary);
                return
                    this.View(new DashBoardModel
                    {
                        Filter = filter,
                        CategoriesExpenseChart = categoriesExpenseChart,
                        CategoriesIncomeChart = categoriesIncomeChart,
                        MonthSummaryChart = monthSummaryChart,
                        YearSummaryChart = yearSummaryChart,
                        Transactions = last5Transactions
                    });
            }
            return this.View();
        }

        #region private

        private LineChart GenerateBarChart(GraphWithDescriptionModel data)
        {
            if (data.GraphData.Count == 0)
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
                            FillColor = "rgba(151,187,205,0.2)",
                            StrokeColor = this._colorGenerator.GenerateColor(),
                            PointColor = ColorGeneratorService.Black,
                            PointStrokeColor = ColorGeneratorService.White,
                            PointHighlightFill = ColorGeneratorService.White,
                            PointHighlightStroke = ColorGeneratorService.Black
                        }
                    }
                }
            };
            lineChart.ChartConfiguration.ScaleBeginAtZero = false;
            lineChart.ChartConfiguration.Responsive = true;
            return lineChart;
        }

        private PieChart GeneratePieChart(List<SimpleGraphModel> result)
        {
            if (result.Count == 0)
            {
                return null;
            }
            return new PieChart
            {
                Data = result.Select(
                    t =>
                        new SimpleData
                        {
                            Label = t.Label,
                            Value = Math.Abs(Convert.ToDouble(t.Value)),
                            Color = this._colorGenerator.GenerateColor()
                        }).ToList()
            };
        }

        #endregion
    }
}