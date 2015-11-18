namespace ExpenseManager.Web.Models.HomePage
{
    /// <summary>
    ///     class for wrapping data returned from queries to graph generator
    /// </summary>
    public class SimpleGraphWrapper
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