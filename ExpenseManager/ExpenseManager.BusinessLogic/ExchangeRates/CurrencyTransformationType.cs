namespace ExpenseManager.BusinessLogic.ExchangeRates
{
    public enum CurrencyTransformationType
    {
        /// <summary>
        ///     Converting from CZK to another currency
        /// </summary>
        FromCzkToOther,

        /// <summary>
        ///     Converting from a currency to CZK
        /// </summary>
        FromOtherToCzk,

        /// <summary>
        ///     Converting between two currencies different from CZK
        /// </summary>
        FromOtherToOther
    }
}