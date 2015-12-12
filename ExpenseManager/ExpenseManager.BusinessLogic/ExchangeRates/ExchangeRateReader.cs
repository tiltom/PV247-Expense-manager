using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using ExpenseManager.BusinessLogic.ServicesConstants;
using ExpenseManager.Resources.ExchangeRateResources;

namespace ExpenseManager.BusinessLogic.ExchangeRates
{
    /// <summary>
    ///     Reads exchange rates from text file at cnb.cz
    /// </summary>
    internal static class ExchangeRateReader
    {
        private static Lazy<Dictionary<Tuple<string, string>, decimal>> _exchangeRatesTable
            = new Lazy<Dictionary<Tuple<string, string>, decimal>>(PreComputeExchangeRateTable);

        private static DateTime _lastCacheUpdate;

        private static Dictionary<Tuple<string, string>, decimal> ExchangeRatesTable
        {
            get
            {
                if (_lastCacheUpdate.Date != DateTime.Now.Date)
                {
                    _exchangeRatesTable =
                        new Lazy<Dictionary<Tuple<string, string>, decimal>>(PreComputeExchangeRateTable);
                    _lastCacheUpdate = DateTime.Now;
                }

                return _exchangeRatesTable.Value;
            }
        }

        public static decimal GetExchangeRate(string fromCurrencyCode, string toCurrencyCode)
        {
            var currencyRateTuple = Tuple.Create(fromCurrencyCode, toCurrencyCode);
            return ExchangeRatesTable.ContainsKey(currencyRateTuple) ? ExchangeRatesTable[currencyRateTuple] : 0;
        }

        #region private

        /// <summary>
        ///     Reads exchange rates from file
        /// </summary>
        /// <returns>List of exchange rate</returns>
        private static List<ExchangeRate> LoadExchangeRates()
        {
            Debug.WriteLine(ExchangeRateResource.ReadingExchangeRates);
            var client = new WebClient();
            var stream = client.OpenRead(ExchangeRateConstant.BasicUrl + DateTime.Now.ToShortDateString());

            if (stream == null)
            {
                throw new WebException(ExchangeRateResource.CouldNotGetData);
            }

            var lines = new List<string>();
            using (var reader = new StreamReader(stream))
            {
                string input;
                while ((input = reader.ReadLine()) != null)
                {
                    lines.Add(input);
                }
            }
            return lines.Skip(ExchangeRateConstant.DescriptionLinesCount).Select(GetExchangeRateFromLine).ToList();
        }

        private static Dictionary<Tuple<string, string>, decimal> PreComputeExchangeRateTable()
        {
            var exchangeRates = LoadExchangeRates();
            // Add Czech Crown to list, because it was not included in parsed document
            exchangeRates.Add(new ExchangeRate
            {
                Code = ExchangeRateConstant.CzkCurrencyCode,
                Rate = 1,
                Amount = 1
            });

            var ratesTable = new Dictionary<Tuple<string, string>, decimal>();

            foreach (var fromCurrency in exchangeRates)
            {
                foreach (var toCurrency in exchangeRates)
                {
                    ratesTable[Tuple.Create(fromCurrency.Code, toCurrency.Code)]
                        = GetExchangeRate(fromCurrency, toCurrency);
                }
            }

            return ratesTable;
        }

        /// <summary>
        ///     Get exchange rate (fromCurrency, toCurrency)
        /// </summary>
        /// <param name="fromCurrency"> Currency from which is exchange rate calculated</param>
        /// <param name="toCurrency"> Currency to which is exchange rate calculated </param>
        /// <summary>
        ///     First the <paramref name="fromCurrency" /> is converted to CZK
        ///     Than the CZK rate is converted to <paramref name="toCurrency" />
        /// </summary>
        /// <returns>Exchange rate between currencies</returns>
        private static decimal GetExchangeRate(ExchangeRate fromCurrency, ExchangeRate toCurrency)
        {
            if (fromCurrency.Code == toCurrency.Code)
            {
                return ExchangeRateConstant.DefaultExchangeRate;
            }

            var czkRate = fromCurrency.Rate/fromCurrency.Amount;
            var toCurrencyRate = toCurrency.Rate*toCurrency.Amount;
            return czkRate/toCurrencyRate;
        }

        private static ExchangeRate GetExchangeRateFromLine(string exchangeRateLine)
        {
            var fields = exchangeRateLine.Split(ExchangeRateConstant.ExchangeRateLineSeparator);

            return new ExchangeRate
            {
                Amount = Convert.ToInt32(fields[2]),
                Code = fields[3],
                Rate =
                    Convert.ToDecimal(fields[4],
                        new NumberFormatInfo
                        {
                            NumberDecimalSeparator = ExchangeRateConstant.DefaultNumberDecimalSeparator
                        })
            };
        }

        #endregion
    }
}