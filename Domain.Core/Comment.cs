using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Core
{
    public class Comment : Entity
    {
        [StringLength(220, MinimumLength = 2)]
        public string Text { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
        
        public int LotId { get; set; }
        
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}