using System;
using System.ComponentModel.DataAnnotations;

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
        public TimeSpan Frequency { get; set; }

        /// <summary>
        ///     Date until which transaction should repeat
        /// </summary>
        public DateTime LastOccurence { get; set; }

        /// <summary>
        ///     First occurrence of transaction, which is then repeated
        /// </summary>
        [Required]
        public virtual Transaction FirstTransaction { get; set; }
    }
}