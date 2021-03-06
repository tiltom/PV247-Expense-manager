﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Transactions;

namespace ExpenseManager.Entity.Wallets
{
    public class Wallet : BaseEntity
    {
        /// <summary>
        ///     Entity representing wallet.
        ///     There is just single wallet per user and it should be created automatically with user.
        ///     Owner can change access rights to his wallet which are represented by <see cref="WalletAccessRight" />
        /// </summary>
        public Wallet()
        {
            WalletAccessRights = new List<WalletAccessRight>();
            Transactions = new List<Transaction>();
        }

        /// <summary>
        ///     Name of wallet
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Currency of wallet. This currency will be used as default currency for budgets and transactions
        /// </summary>
        [Required]
        public virtual Currency Currency { get; set; }

        /// <summary>
        ///     Collection of access rights to this wallet
        /// </summary>
        public virtual ICollection<WalletAccessRight> WalletAccessRights { get; set; }

        /// <summary>
        ///     Collection of transactions made in this wallet
        /// </summary>
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}