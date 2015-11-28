using System.Collections.Generic;
using System.Linq;
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
        ///     Income categories chart data
        /// </summary>
        public PieChart CategoriesIncomeChart { get; set; }

        /// <summary>
        ///     Exepense categories chart data
        /// </summary>
        public PieChart CategoriesExpenseChart { get; set; }


        /// <summary>
        ///     last month chart data
        /// </summary>
        public LineChart MonthSummaryChart { get; set; }


        /// <summary>
        ///     last year chart data
        /// </summary>
        public LineChart YearSummaryChart { get; set; }

        /// <summary>
        ///     filter data model
        /// </summary>
        public FilterDataModel Filter { get; set; }

        /// <summary>
        ///     last 5 transactions
        /// </summary>
        public IEnumerable<TransactionShowModel> Transactions { get; set; }


        public bool ContainsGraphData()
        {
            return MonthSummaryChart != null
                   && YearSummaryChart != null
                   && CategoriesExpenseChart != null
                   && CategoriesIncomeChart != null;
        }

        public bool ContainsAnyData()
        {
            return this.ContainsGraphData() && Transactions.Any();
        }
    }
}