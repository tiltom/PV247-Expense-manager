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

            var transformationType = GetCurrencyTransformationType(transaction.Currency, walletCurrency);

            switch (transformationType)
            {
                case CurrencyTransformationType.FromCzkToOther:
                    ConvertCurrencyFromCzk(transaction, walletCurrency, rates);
                    break;

                case CurrencyTransformationType.FromOtherToCzk:
                    ConvertCurrencyToCzk(transaction, rates);
                    break;

                case CurrencyTransformationType.FromOtherToOther:
                    ConvertCurrencies(transaction, walletCurrency, rates);
                    break;
            }

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
            ConvertCurrencyToCzk(transaction, rates);
            ConvertCurrencyFromCzk(transaction, currency, rates);
        }

        private static CurrencyTransformationType GetCurrencyTransformationType(Currency fromCurrency,
            Currency toCurrency)
        {
            if (fromCurrency.Symbol == CzechCurrencySymbol)
            {
                return CurrencyTransformationType.FromCzkToOther;
            }

            if (toCurrency.Symbol == CzechCurrencySymbol)
            {
                return CurrencyTransformationType.FromOtherToCzk;
            }

            return CurrencyTransformationType.FromOtherToOther;
        }

        private static string GetCurrencyCode(Currency currency)
        {
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

        #endregion
    }
}