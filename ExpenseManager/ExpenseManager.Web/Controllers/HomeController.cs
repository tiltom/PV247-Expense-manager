using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Chart.Mvc.SimpleChart;
using ExpenseManager.Database;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Web.Models.HomePage;

namespace ExpenseManager.Web.Controllers
{
    [RequireHttps]
    public class HomeController : AbstractController
    {
        public const string ChartData = "ChartData";
        // TODO make some color generator :/
        private static readonly string[] ColourValues =
        {
            "FF0000", "00FF00", "0000FF", "FFFF00", "FF00FF", "00FFFF", "000000",
            "800000", "008000", "000080", "808000", "800080", "008080", "808080",
            "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0",
            "400000", "004000", "000040", "404000", "400040", "004040", "404040",
            "200000", "002000", "000020", "202000", "200020", "002020", "202020",
            "600000", "006000", "000060", "606000", "600060", "006060", "606060",
            "A00000", "00A000", "0000A0", "A0A000", "A000A0", "00A0A0", "A0A0A0",
            "E00000", "00E000", "0000E0", "E0E000", "E000E0", "00E0E0", "E0E0E0"
        };

        private readonly ITransactionsProvider _transactionsProvider = ProvidersFactory.GetNewTransactionsProviders();

        public ViewResult Index()
        {
            return this.View(new FilterDataModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(FilterDataModel model)
        {
            var currentUser = await this.CurrentProfileId();
            var result = await this._transactionsProvider.Transactions
                .Where(
                    t =>
                        t.Date <= model.EndDate && t.Date >= model.StartDate &&
                        (t.Wallet.WalletAccessRights.Any(war => war.UserProfile.Guid == currentUser) ||
                         t.Budget.AccessRights.Any(bar => bar.UserProfile.Guid == currentUser)
                            )
                )
                .GroupBy(t => t.Category.Name)
                .Select(
                    x =>
                        new
                        {
                            x = x.Key,
                            y = x.Sum(f => f.Amount)
                        })
                .ToListAsync();

            var pieChart = new PieChart();
            pieChart.Data.AddRange(result.Select(
                (t, i) =>
                    new SimpleData
                    {
                        Label = t.x,
                        Value = Convert.ToDouble(t.y),
                        Color = "#" + ColourValues[i]
                    }));
            ViewBag.ChartData = pieChart;
            return this.View(model);
        }
    }
}