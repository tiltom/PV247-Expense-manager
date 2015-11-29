namespace ExpenseManager.BusinessLogic.ExchangeRates
{
    /// <summary>
    ///     Represents exchange rate between the currency and CZK
    /// </summary>
    public class ExchangeRate
    {
        /// <summary>
        ///     How much of this currency needs to be calculated
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        ///     Code of the currency, for example GBP, USD, EUR
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     Exchange rate to this currency
        /// </summary>
        public decimal Rate { get; set; }
    }
}