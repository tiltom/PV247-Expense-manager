﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ExpenseManager.Entity.Enums;

namespace ExpenseManager.Web.Models.Transaction
{
    /// <summary>
    ///     Model for adding new transaction
    /// </summary>
    public class NewTransactionModel
    {
        /// <summary>
        ///     Amount of money in transaction
        /// </summary>
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        ///     Date when transaction occurred
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        /// <summary>
        ///     Short description of transaction
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Id of wallet where transaction belongs
        /// </summary>
        [Required]
        public Guid WalletId { get; set; }

        /// <summary>
        ///     Id of budget where transaction belongs
        /// </summary>
        [Display(Name = "Budget")]
        public Guid? BudgetId { get; set; }

        /// <summary>
        ///     Id of currency which was used for transaction
        /// </summary>
        [Display(Name = "Currency")]
        [Required]
        public Guid CurrencyId { get; set; }

        /// <summary>
        ///     Id of category where transaction belongs
        /// </summary>
        [Display(Name = "Category")]
        [Required]
        public Guid CategoryId { get; set; }

        /// <summary>
        ///     Bool representing if transaction is repeatable
        /// </summary>
        [Display(Name = "Repeat transaction?")]
        public bool IsRepeatable { get; set; }

        /// <summary>
        ///     How often should transaction repeat
        /// </summary>
        [Display(Name = "Frequency")]
        public int NextRepeat { get; set; }

        /// <summary>
        ///     Type of repetition
        /// </summary>
        [Display(Name = "Repetition")]
        public FrequencyType FrequencyType { get; set; }

        /// <summary>
        ///     Date until which transaction should repeat
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Last Occurrence")]
        public DateTime? LastOccurrence { get; set; }

        /// <summary>
        ///     List of all available Categories
        /// </summary>
        public List<SelectListItem> Categories { get; set; }

        /// <summary>
        ///     List of all available Currencies
        /// </summary>
        public List<SelectListItem> Currencies { get; set; }

        /// <summary>
        ///     List of all available Budgets
        /// </summary>
        public List<SelectListItem> Budgets { get; set; }
    }
}