namespace ExpenseManager.BusinessLogic.ServicesConstants
{
    internal class ExchangeRateConstant
    {
        public const string BasicUrl =
            "http://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.txt?date=";

        public const int DescriptionLinesCount = 2;
        public const string CzkCurrencyCode = "CZK";
        public const int DefaultExchangeRate = 1;
        public const char ExchangeRateLineSeparator = '|';
        public const string DefaultNumberDecimalSeparator = ",";
    }
}