using System.Collections.Generic;
using System.Linq;

namespace ExpenseManager.BusinessLogic.DashboardServices.Models
{
    /// <summary>
    ///     class for wrapping data with multiple values from graph generator
    /// </summary>
    internal class MultiValueGraphModel
    {
        public MultiValueGraphModel()
        {
            Values = Enumerable.Empty<decimal>();
        }

        /// <summary>
        ///     label of property
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        ///     value of property
        /// </summary>
        public IEnumerable<decimal> Values { get; set; }
    }
}