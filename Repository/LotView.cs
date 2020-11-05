using System;
using System.Collections.Generic;
using System.Linq;
using Data;

namespace Repository
{
    public class LotView
    {
        public int Id { get; set; }
        
        public string ImageUrl { get; set; }
        
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime LunchAt { get; set; }
        
        public DateTime EndAt { get; set; }
        
        public decimal Goal { get; set; }
        public decimal Funded { get; set; }
        
        public int BetCount { get; set; }
        
        public Category Category { get; set; }

        public string AppUserId { get; set; }
        public string UserName { get; set; }

        public bool IsAvailable { get; set; }

        public LotView(Lot lot)
        {
            Id = lot.Id;
            ImageUrl = lot.ImageUrl;
            Title = lot.Title;
            Description = lot.Description;
            LunchAt = lot.LunchAt;
            EndAt = lot.EndAt;
            Goal = lot.Goal;
            Funded = lot.Rates?.OrderByDescending(c => c.CreatedAt).FirstOrDefault()?.Amount ?? 0m;
            BetCount = lot.Rates?.Count ?? 0;
            Category = lot.Category;
            AppUserId = lot.AppUserId;
            IsAvailable = lot.IsAvailable;
        }
    }
}