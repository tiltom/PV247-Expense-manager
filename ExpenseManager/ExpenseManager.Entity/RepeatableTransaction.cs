using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManager.Entity
{
    public class RepeatableTransaction : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public TimeSpan Frequency { get; set; }
        public DateTime LastOccurence { get; set; }
        public int FirstTransactionId { get; set; }
        public virtual Transaction FirstTransaction { get; set; }
    }
}