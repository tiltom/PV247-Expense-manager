using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace ExpenseManager.BusinessLogic.ExchangeRates
{
    /// <summary>
    ///     Reads exchange rates from text file at cnb.cz
    /// </summary>
    internal class ExchangeRateReader
    {
        public const string BasicUrl =
            "http://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.txt?date=";

        /// <summary>
        ///     Reads exchange rates from file
        /// </summary>
        /// <returns>List of exchange rate</returns>
        public List<ExchangeRate> LoadExchangeRates(DateTime date)
        {
            var lines = new List<string>();

            var client = new WebClient();
            var stream = client.OpenRead(this.ConstructUrl(date));
            var reader = new StreamReader(stream);

            string input;

            // first line defines date, we dont need thid
            reader.ReadLine();
            // this is the line with the description, not data, so skip it
            reader.ReadLine();

            while ((input = reader.ReadLine()) != null)
            {
                lines.Add(input);
            }

            return lines.Select(this.GetExchangeRateFromString).ToList();
        }

        #region private

        private string ConstructUrl(DateTime date)
        {
            return BasicUrl + date.ToShortDateString();
        }

        private ExchangeRate GetExchangeRateFromString(string exchangeRate)
        {
            var fields = exchangeRate.Split('|');

            return new ExchangeRate
            {
                Amount = Convert.ToInt32(fields[2]),
                Code = fields[3],
                Rate = Convert.ToDecimal(fields[4])
            };
        }

        #endregion
    }
}