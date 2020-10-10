using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Auction.Models
{
    public class Lot : Entity
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Story { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime LunchAt { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime EndAt { get; set; }
        
        public decimal Goal { get; set; }
        public decimal Funded { get; set; }
        
        public List<Rate> Rates { get; set; }
        
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}