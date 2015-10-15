using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Entity
{
    public class RepeatableTransaction : BaseEntity
    {
        public TimeSpan Frequency { get; set; }
        public DateTime LastOccurence { get; set; }

        [Required]
        public virtual Transaction FirstTransaction { get; set; }
    }
}