using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Core
{
    public class Lot : Entity
    {
        public string ImageUrl { get; set; }
        
        [RegularExpression(@"^[^\@]")]
        [MaxLength(60)]
        public string Title { get; set; }
        
        [DataType(DataType.Text)]
        [MaxLength(135)]
        public string Description { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string Story { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime LunchAt { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime EndAt { get; set; }
        
        [RegularExpression("^[0-9]+")]
        public decimal MinPrice { get; set; }
        
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        
        public List<Rate> Rates { get; set; }
        public List<Comment> Comments { get; set; }
        
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        
        public bool IsAvailable { get; set; }
    }
}