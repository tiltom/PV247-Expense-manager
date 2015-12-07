namespace ExpenseManager.BusinessLogic.TransactionServices.Models
{
    public class TransactionServiceExportModel : TransactionShowServiceModel
    {
        /// <summary>
        ///     Official code of currency (e.g. USD, EUR or CZK)
        /// </summary>
        public string CurrencyCode { get; set; }
    }
}