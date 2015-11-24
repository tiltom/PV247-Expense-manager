using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            var categories = await this._dashBoardService.GetWrapperValuesForCategories(resultMonth);
            var monthSummary = await this._dashBoardService.GetGraphForDaysLastMonth(resultMonth);
            var yearSummary = await this._dashBoardService.GetGraphForMonthLastYear(resultYear);
            // generate pie chart
            var categoriesChart = this.GeneratePieChart(categories);
            var monthSummaryChart = this.GenerateBarChart(monthSummary);
            var yearSummaryChart = this.GenerateBarChart(yearSummary);
            return
                this.View(new DashBoardModel
                {
                    Filter = filter,
                    CategoriesChart = categoriesChart,
                    MonthSummaryChart = monthSummaryChart,
                    YearSummaryChart = yearSummaryChart,
                    Transactions = last5Transactions
                });
        }

        #region private

        private BarChart GenerateBarChart(GraphWithDescriptionModel data)
        {
            var barChart = new BarChart();
            barChart.ComplexData.Labels.AddRange(data.GraphData.Select(t => t.Label));
            barChart.ComplexData.Datasets.AddRange(new List<ComplexDataset>
            {
                new ComplexDataset
                {
                    Data = data.GraphData.Select(t => Convert.ToDouble(t.Value)).ToList(),
                    Label = data.Description,
                    FillColor = this._colorGenerator.GenerateColor(),
                    StrokeColor = ColorGeneratorService.Black,
                    PointColor = ColorGeneratorService.Black,
                    PointStrokeColor = ColorGeneratorService.Black,
                    PointHighlightFill = ColorGeneratorService.Black,
                    PointHighlightStroke = ColorGeneratorService.Black
                }
            });
            return barChart;
        }

        private PieChart GeneratePieChart(List<SimpleGraphModel> result)
        {
            var pieChart = new PieChart();
            pieChart.Data.AddRange(result.Select(
                t =>
                    new SimpleData
                    {
                        Label = t.Label,
                        Value = Convert.ToDouble(t.Value),
                        Color = this._colorGenerator.GenerateColor()
                    }));
            return pieChart;
        }

        #endregion
    }
}