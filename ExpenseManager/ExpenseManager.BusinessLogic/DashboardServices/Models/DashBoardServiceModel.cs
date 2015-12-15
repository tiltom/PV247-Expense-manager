using System.Collections.Generic;
using System.Linq;
using Chart.Mvc.ComplexChart;
using Chart.Mvc.SimpleChart;
using ExpenseManager.Entity.Transactions;

namespace ExpenseManager.BusinessLogic.DashboardServices.Models
{
    public class DashBoardServiceModel
    {
        public DashBoardServiceModel()
        {
            Filter = new FilterDataServiceModel();
            Transactions = Enumerable.Empty<Transaction>();
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
        public FilterDataServiceModel Filter { get; set; }

        /// <summary>
        ///     last transactions (number is defined in constant DashBoardService.NumberOfTransactionsOnDashBoard)
        /// </summary>
        public IEnumerable<Transaction> Transactions { get; set; }

        /// <summary>
        ///     Chart displaying used limits in all budgets
        /// </summary>
        public BarChart BudgetLimitChart { get; set; }
    }
}