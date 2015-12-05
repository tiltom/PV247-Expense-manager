using ExpenseManager.Entity.Budgets;
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

        /// <summary>
        ///     Changes limit of the budget according to the wallet currency
        /// </summary>
        /// <param name="budget">Budget to change limit</param>
        /// <param name="walletCurrency">Currency of a wallet, according to this currency will be the transaction currency changed</param>
        /// <param name="oldCurrency">Old currency of a wallet</param>
        public static void ChangeCurrency(Budget budget, Currency walletCurrency, Currency oldCurrency)
        {
            budget.Limit = budget.Limit*
                           ExchangeRateReader.GetExchangeRate(oldCurrency.Code, walletCurrency.Code);
        }
    }
}