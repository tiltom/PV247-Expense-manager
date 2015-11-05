namespace ExpenseManager.Entity.Currencies
{
    public class Currency : BaseEntity
    {
        /// <summary>
        ///     Name of currency
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Symbol of currency (e.g. $ or Kč)
        /// </summary>
        public string Symbol { get; set; }
    }
}