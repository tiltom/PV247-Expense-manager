using System;
using System.ComponentModel.DataAnnotations;
using ExpenseManager.Entity.Enums;

namespace ExpenseManager.Entity.Transactions
{
    /// <summary>
    ///     Entity for representing repetition of transaction
    /// </summary>
    public class RepeatableTransaction : BaseEntity
    {
        /// <summary>
        ///     How often should transaction repeat
        /// </summary>
        public int NextRepeat { get; set; }

        public FrequencyType FrequencyType { get; set; }

        /// <summary>
        ///     Type of transaction repetition
        /// </summary>
        /// <summary>
        ///     Date until which transaction should repeat
        /// </summary>
        public DateTime LastOccurrence { get; set; }

        /// <summary>
        ///     First occurrence of transaction, which is then repeated
        /// </summary>
        [Required]
        public virtual Transaction FirstTransaction { get; set; }
    }
}