using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    public class Rate : Entity
    {
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        
        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        
        public string AppUserId { get; set; }
        public int LotId { get; set; }
    }
}