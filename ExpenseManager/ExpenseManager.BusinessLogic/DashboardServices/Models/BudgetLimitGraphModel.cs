using System.Collections.Generic;
using System.Linq;

namespace ExpenseManager.BusinessLogic.DashboardServices.Models
{
    /// <summary>
    ///     class for wrapping data with more labels and  multiple values from graph
    /// </summary>
    internal class BudgetLimitGraphModel
    {
        public BudgetLimitGraphModel()
        {
            Values = Enumerable.Empty<BudgetWithLimitGraphModel>();
        }

        /// <summary>
        ///     label of property
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        ///     value of property
        /// </summary>
        public IEnumerable<BudgetWithLimitGraphModel> Values { get; set; }
    }
}