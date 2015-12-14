using System.Collections.Generic;
using System.Linq;

namespace ExpenseManager.BusinessLogic.DashboardServices.Models
{
    /// <summary>
    ///     class for wrapping data with more labels and  multiple values from graph
    /// </summary>
    internal class ComplexGraphModel
    {
        public ComplexGraphModel()
        {
            Values = Enumerable.Empty<MultiValueGraphModel>();
        }

        /// <summary>
        ///     label of property
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        ///     value of property
        /// </summary>
        public IEnumerable<MultiValueGraphModel> Values { get; set; }
    }
}