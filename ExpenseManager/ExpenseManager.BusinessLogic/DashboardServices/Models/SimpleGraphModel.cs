namespace ExpenseManager.BusinessLogic.DashboardServices.Models
{
    /// <summary>
    ///     class for wrapping data returned from queries to graph generator
    /// </summary>
    public class SimpleGraphModel
    {
        /// <summary>
        ///     label of property
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        ///     value of property
        /// </summary>
        public decimal Value { get; set; }
    }
}