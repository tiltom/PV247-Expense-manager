using Chart.Mvc.ComplexChart;
using Chart.Mvc.SimpleChart;

namespace ExpenseManager.Web.Models.HomePage
{
    /// <summary>
    ///     Model representing gome page
    /// </summary>
    public class DashBoardModel
    {
        public DashBoardModel()
        {
            Filter = new FilterDataModel();
        }

        /// <summary>
        ///     categories chart data
        /// </summary>
        public PieChart Categories { get; set; }

        /// <summary>
        ///     months chart data
        /// </summary>
        public BarChart Months { get; set; }

        /// <summary>
        ///     filter data model
        /// </summary>
        public FilterDataModel Filter { get; set; }
    }
}