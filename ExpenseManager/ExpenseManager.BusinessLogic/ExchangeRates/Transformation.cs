using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Transactions;

namespace ExpenseManager.BusinessLogic.ExchangeRates
{
    public class Transformation
    {
        /// <summary>
        ///     Changes currency (and amount) of the transaction according to the wallet currency
        /// </summary>
        /// <param name="transaction">Transaction of a changed currency</param>
        /// <param name="walletCurrency">Currency of a wallet, according to this currency will be the transaction currency changed</param>
        public static void ChangeCurrency(Transaction transaction, Currency walletCurrency)
        {
            transaction.Amount = transaction.Amount*
                                 ExchangeRateReader.GetExchangeRate(transaction.Currency.Code, walletCurrency.Code);
            transaction.Currency = walletCurrency;
        }
    }
}