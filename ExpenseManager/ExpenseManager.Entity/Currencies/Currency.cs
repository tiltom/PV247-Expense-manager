﻿namespace ExpenseManager.Entity.Currencies
{
    public class Currency : BaseEntity
    {
        /// <summary>
        ///     Name of currency
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Symbol of currency (e.g. $ or Kč)
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        ///     Official code of currency (e.g. USD, EUR or CZK)
        /// </summary>
        public string Code { get; set; }
    }
}