namespace ExpenseManager.BusinessLogic.DashboardServices.Models
{
    /// <summary>
    ///     class for wrapping data with multiple values from graph generator
    /// </summary>
    internal class BudgetWithLimitGraphModel
    {
        /// <summary>
        ///     label of property
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        ///     value of property
        /// </summary>
        public decimal BudgetLimit { get; set; }

        /// <summary>
        ///     value of property
        /// </summary>
        public decimal ComputedTransaction { get; set; }
    }
}