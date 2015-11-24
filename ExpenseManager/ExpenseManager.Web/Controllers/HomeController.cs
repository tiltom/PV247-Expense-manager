using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Chart.Mvc.ComplexChart;
using Chart.Mvc.SimpleChart;
using ExpenseManager.BusinessLogic.CommonServices;
using ExpenseManager.BusinessLogic.DashboardServices;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Web.Models.HomePage;

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
        public ViewResult Index()
        {
            return this.View(new DashBoardModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(FilterDataModel filter)
        {
            var model = new DashBoardModel();
            // select transactions according to filter
            var result = await this.FilterTransactions(filter);
            // prepare data for graphs
            var categories = await GetWrapperValuesForCategories(result);
            var months = await GetWrapperValuesForMonths(result);
            // generate pie chart
            model.Categories = this.GeneratePieChart(categories);
            model.Months = this.GenerateBarChart(months);
            model.Filter = filter;
            return this.View(model);
        }

        #region private

        private BarChart GenerateBarChart(List<SimpleGraphWrapper> months)
        {
            var barChart = new BarChart();
            barChart.ComplexData.Labels.AddRange(months.Select(t => t.Label));
            barChart.ComplexData.Datasets.AddRange(new List<ComplexDataset>
            {
                new ComplexDataset
                {
                    Data = months.Select(t => Convert.ToDouble(t.Value)).ToList(),
                    Label = "Monthly report",
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

        private static async Task<List<SimpleGraphWrapper>> GetWrapperValuesForCategories(IQueryable<Transaction> result)
        {
            return await result
                .GroupBy(t => t.Category.Name)
                .Select(
                    x =>
                        new SimpleGraphWrapper
                        {
                            Label = x.Key,
                            Value = x.Sum(f => f.Amount > 0 ? f.Amount : f.Amount*-1)
                        })
                .ToListAsync();
        }


        private static async Task<List<SimpleGraphWrapper>> GetWrapperValuesForMonths(IQueryable<Transaction> result)
        {
            return await result
                .GroupBy(t => t.Date.Month)
                .Select(
                    x =>
                        new SimpleGraphWrapper
                        {
                            Label = x.Key.ToString(),
                            Value = x.Sum(f => f.Amount)
                        })
                .ToListAsync();
        }

        private async Task<IQueryable<Transaction>> FilterTransactions(FilterDataModel model)
        {
            var currentUser = await this.CurrentProfileId();
            var result = this._dashBoardService.GetAccessibleResults(currentUser)
                .Where(
                    t =>
                        t.Date <= model.EndDate && t.Date >= model.StartDate
                        && (!model.Budgets.Any() || model.Budgets.Contains(t.Budget.Guid))
                        && (!model.Wallets.Any() || model.Wallets.Contains(t.Wallet.Guid))
                        && (!model.Categories.Any() || model.Categories.Contains(t.Category.Guid))
                );
            return result;
        }

        private PieChart GeneratePieChart(List<SimpleGraphWrapper> result)
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