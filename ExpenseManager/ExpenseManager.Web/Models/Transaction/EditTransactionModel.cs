﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Web.Models.Transaction
{
    /// <summary>
    ///     Model for editing transaction, extends model for adding transaction
    /// </summary>
    public class EditTransactionModel : NewTransactionModel
    {
        /// <summary>
        ///     Unique id of transaction
        /// </summary>
        [Required]
        public Guid Id { get; set; }
    }
}