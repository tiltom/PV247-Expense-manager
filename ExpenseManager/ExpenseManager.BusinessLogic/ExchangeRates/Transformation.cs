using System;
using System.Collections.Generic;
using System.Linq;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Transactions;

namespace ExpenseManager.BusinessLogic.ExchangeRates
{
    public class Transformation
    {
        public const string CzechCurrencySymbol = "Kč";

        /// <summary>
        ///     Changes currency (and amount) of the transaction according to the wallet currency
        /// </summary>
        /// <param name="transaction">Transaction of a changed currency</param>
        /// <param name="walletCurrency">Currency of a wallet, according to this currency will be the transaction currency changed</param>
        public static void ChangeCurrency(Transaction transaction, Currency walletCurrency)
        {
            var reader = new ExchangeRateReader();
            var rates = reader.LoadExchangeRates(transaction.Date);

            ConvertCurrencies(transaction, walletCurrency, rates);

            transaction.Currency = walletCurrency;
        }

        #region private

        private static void ConvertCurrencyFromCzk(Transaction transaction, Currency currency, List<ExchangeRate> rates)
        {
            var walletCurrencyCode = GetCurrencyCode(currency);
            var rate = rates.FirstOrDefault(x => x.Code == walletCurrencyCode);
            transaction.Amount = (transaction.Amount*rate.Amount)/rate.Rate;
        }

        private static void ConvertCurrencyToCzk(Transaction transaction, List<ExchangeRate> rates)
        {
            var transactionCurrencyCode = GetCurrencyCode(transaction.Currency);
            var rate = rates.FirstOrDefault(x => x.Code == transactionCurrencyCode);
            transaction.Amount = (transaction.Amount/rate.Amount)*rate.Rate;
        }

        private static void ConvertCurrencies(Transaction transaction, Currency currency, List<ExchangeRate> rates)
        {
            if (!IsCzkCurrency(transaction.Currency))
            {
                ConvertCurrencyToCzk(transaction, rates);
            }

            if (!IsCzkCurrency(currency))
            {
                ConvertCurrencyFromCzk(transaction, currency, rates);
            }
        }

        private static string GetCurrencyCode(Currency currency)
        {
            // TODO: change this, so the code and symbols are in db
            switch (currency.Symbol)
            {
                case "€":
                    return "EUR";
                case "$":
                    return "USD";
                case "£":
                    return "GBP";
                case "Kč":
                    return "CZK";
            }

            throw new NotSupportedException("This currency is not supported!");
        }

        private static bool IsCzkCurrency(Currency currency)
        {
            return currency.Symbol.Equals(CzechCurrencySymbol);
        }

        #endregion
    }
}