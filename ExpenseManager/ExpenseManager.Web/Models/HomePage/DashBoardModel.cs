using System.Collections.Generic;
using Chart.Mvc.ComplexChart;
using Chart.Mvc.SimpleChart;
using ExpenseManager.Web.Models.Transaction;

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
        public PieChart CategoriesChart { get; set; }


        /// <summary>
        ///     last month chart data
        /// </summary>
        public BarChart MonthSummaryChart { get; set; }


        /// <summary>
        ///     last year chart data
        /// </summary>
        public BarChart YearSummaryChart { get; set; }

        /// <summary>
        ///     filter data model
        /// </summary>
        public FilterDataModel Filter { get; set; }

        /// <summary>
        ///     last 5 transactions
        /// </summary>
        public IEnumerable<TransactionShowModel> Transactions { get; set; }
    }
}